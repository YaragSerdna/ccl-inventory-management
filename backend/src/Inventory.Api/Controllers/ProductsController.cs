using Inventory.Api.Data;
using Inventory.Api.DTOs;
using Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Inventory.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de la clase ProductsController.
        /// </summary>
        /// <param name="context">Contexto de base de datos de inventario.</param>
        public ProductsController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        /// <summary>
        /// Recupera una lista de productos activos ordenados por nombre.
        /// </summary>
        /// <returns>Un resultado de acción que contiene la lista de productos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Name).Select(
                p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.Description,
                    p.Stock,
                    p.UnitPrice,
                    p.IsActive
                }).ToListAsync();

            return Ok(products);
        }

        // GET: api/products/1
        /// <summary>
        /// Recupera un producto activo por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a recuperar.</param>
        /// <returns>Un resultado de acción que contiene el producto o un error 404 si no se encuentra.</returns>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var product = await _context.Products.AsNoTracking().Where(p => p.Id == id && p.IsActive).Select(
                p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.Description,
                    p.Stock,
                    p.UnitPrice,
                    p.IsActive
                }).FirstOrDefaultAsync();

            if (product is null)
            {
                return NotFound(new { message = $"No se encontró un producto activo con el id {id}." });
            }

            return Ok(product);
        }

        // POST: api/products
        /// <summary>
        /// Crea un nuevo producto y lo agrega a la base de datos.
        /// </summary>
        /// <param name="dto">Datos para crear el producto.</param>
        /// <returns>Un resultado que indica el estado de la operación: 201 Created si se crea correctamente, 409 Conflict si el código ya existe.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductDto dto)
        {
            var code = dto.Code.Trim();

            var codeExists = await _context.Products.AnyAsync(p => p.Code == code);

            if (codeExists)
            {
                return Conflict(new { message = $"Ya existe un producto con el código '{code}'." });
            }

            var product = new Product
            {
                Code = code,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                UnitPrice = dto.UnitPrice,
                Stock = 0,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.GetBaseException() is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict(new { message = $"Ya existe un producto con el código '{code}'." });
            }

            return CreatedAtAction(nameof(GetProduct),new { id = product.Id },
                new
                {
                    product.Id,
                    product.Code,
                    product.Name,
                    product.Description,
                    product.Stock,
                    product.UnitPrice,
                    product.IsActive
                });
        }

        // PUT: api/products/1
        /// <summary>
        /// Actualiza un producto activo existente con los datos proporcionados.
        /// </summary>
        /// <param name="id">Identificador del producto a actualizar.</param>
        /// <param name="dto">Datos para actualizar el producto.</param>
        /// <returns>Código de estado HTTP 204 si la actualización es exitosa; 404 si no se encuentra el producto; 409 si el código ya existe.</returns>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product is null)
            {
                return NotFound(new { message = $"No se encontró un producto activo con el id {id}." });
            }

            var code = dto.Code.Trim();

            var codeExists = await _context.Products.AnyAsync(p => p.Code == code && p.Id != id);

            if (codeExists)
            {
                return Conflict(new { message = $"Ya existe otro producto con el código '{code}'." });
            }

            product.Code = code;
            product.Name = dto.Name.Trim();
            product.Description = dto.Description?.Trim();
            product.UnitPrice = dto.UnitPrice;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.GetBaseException() is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict(new { message = $"Ya existe otro producto con el código '{code}'." });
            }

            return NoContent();
        }

        // DELETE: api/products/1
        /// <summary>
        /// Desactiva lógicamente un producto activo por identificador.Desactivación lógica: conserva el registro y su historial.
        /// </summary>
        /// <remarks>La desactivación lógica conserva el registro y su historial.</remarks>
        /// <param name="id">Identificador del producto a desactivar.</param>
        /// <returns>Código de estado 204 si se desactiva correctamente; 404 si no se encuentra un producto activo con el identificador especificado.</returns>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeactivateProduct(long id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product is null)
            {
                return NotFound(new { message = $"No se encontró un producto activo con el id {id}." });
            }

            product.IsActive = false;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
