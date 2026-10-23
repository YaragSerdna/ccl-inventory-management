using Inventory.Api.DTOs.Inventory;
using Inventory.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Inventory.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase InventoryController.
        /// </summary>
        /// <param name="inventoryService">Servicio de inventario utilizado para las operaciones de inventario.</param>
        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //Post: /api/inventory/entries
        /// <summary>
        /// Registra una nueva entrada de inventario.
        /// </summary>
        /// <param name="dto">Datos para registrar la entrada de inventario.</param>
        /// <returns>Resultado de la operación de registro de entrada.</returns>
        [HttpPost("entries")]
        public async Task<IActionResult> RegisterEntry([FromBody] RegisterInventoryMovementDto dto)
        {
            try
            {
                await _inventoryService.RegisterEntryAsync(dto);

                return Ok(new { message = "Entrada de inventario registrada correctamente." });
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.RaiseException)
            {
                return BadRequest(new { message = ex.MessageText });
            }
        }

        //Post: /api/inventory/exits
        /// <summary>
        /// Registra una nueva salida de inventario.
        /// </summary>
        /// <param name="dto">Datos para registrar la salida de inventario.</param>
        /// <returns>Resultado de la operación de registro de salida.</returns>
        [HttpPost("exits")]
        public async Task<IActionResult> RegisterExit([FromBody] RegisterInventoryMovementDto dto)
        {
            try
            {
                await _inventoryService.RegisterExitAsync(dto);

                return Ok(new { message = "Salida de inventario registrada correctamente." });
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.RaiseException)
            {
                return BadRequest(new { message = ex.MessageText });
            }
        }

        //Get: /api/inventory/movements?productId={productId}
        /// <summary>
        /// Obtiene los movimientos de inventario asociados a un producto específico o todos los movimientos si no se
        /// especifica un identificador de producto.
        /// </summary>
        /// <param name="productId">Identificador opcional del producto para filtrar los movimientos.</param>
        /// <returns>Una respuesta HTTP que contiene la lista de movimientos de inventario.</returns>
        [HttpGet("movements")]
        public async Task<IActionResult> GetMovements([FromQuery] long? productId = null)
        {
            if (productId.HasValue && productId.Value <= 0)
            {
                return BadRequest(new { message = "El identificador del producto debe ser mayor que cero." });
            }

            var movements = await _inventoryService.GetMovementsAsync(productId);

            return Ok(movements);
        }

        //Get: /api/inventory/current
        /// <summary>
        /// Obtiene el inventario actual disponible.
        /// </summary>
        /// <returns>Un resultado de acción que contiene la lista de inventario actual.</returns>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentInventory()
        {
            var inventory = await _inventoryService.GetCurrentInventoryAsync();

            return Ok(inventory);
        }

    }
}