using Inventory.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public HealthController(InventoryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Comprueba la conectividad con la base de datos PostgreSQL y devuelve el estado de la conexión.
        /// </summary>
        /// <returns>Un resultado HTTP que indica si la conexión con la base de datos fue exitosa o no.</returns>

        [HttpGet("database")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    return StatusCode(503, new
                    {
                        status = "error",
                        message = "No fue posible conectar con PostgreSQL."
                    });
                }

                return Ok(new
                {
                    status = "ok",
                    database = "PostgreSQL",
                    message = "Conexión establecida correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(503, new
                {
                    status = "error",
                    message = "No fue posible conectar con la base de datos."
                });
            }
        }
    }
}
