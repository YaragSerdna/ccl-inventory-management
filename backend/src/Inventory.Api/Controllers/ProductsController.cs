using Inventory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
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
    }
}
