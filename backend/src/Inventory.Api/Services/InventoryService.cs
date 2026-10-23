using Inventory.Api.Data;
using Inventory.Api.DTOs.Inventory;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Inventory.Api.Services
{
    public class InventoryService
    {
        private readonly InventoryDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de la clase InventoryService utilizando el contexto de base de datos
        /// proporcionado.
        /// </summary>
        /// <param name="context">Contexto de base de datos de inventario.</param>
        public InventoryService(InventoryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra una entrada de inventario utilizando el procedimiento almacenado correspondiente.
        /// </summary>
        /// <param name="dto">Datos para registrar el movimiento de inventario.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task RegisterEntryAsync(RegisterInventoryMovementDto dto)
        {
            await ExecuteMovementProcedureAsync("CALL public.sp_register_stock_entry(@productId, @quantity, @notes)", dto);
        }

        public async Task RegisterExitAsync(RegisterInventoryMovementDto dto)
        {
            await ExecuteMovementProcedureAsync("CALL public.sp_register_stock_exit(@productId, @quantity, @notes)", dto);
        }

        /// <summary>
        /// Ejecuta un procedimiento de movimiento de inventario utilizando los parámetros proporcionados.
        /// </summary>
        /// <param name="sql">Consulta SQL que representa el procedimiento a ejecutar.</param>
        /// <param name="dto">Datos del movimiento de inventario a registrar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        private async Task ExecuteMovementProcedureAsync(string sql, RegisterInventoryMovementDto dto)
        {
            var connection = _context.Database.GetDbConnection(); var shouldClose = connection.State != System.Data.ConnectionState.Open;

            try
            {
                if (shouldClose)
                    await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = sql;

                var productParameter = new NpgsqlParameter("productId", NpgsqlDbType.Bigint) { Value = dto.ProductId };

                var quantityParameter = new NpgsqlParameter("quantity", NpgsqlDbType.Integer) { Value = dto.Quantity };

                var notesParameter = new NpgsqlParameter("notes", NpgsqlDbType.Varchar) { Value = (object?)dto.Notes ?? DBNull.Value };

                command.Parameters.Add(productParameter);
                command.Parameters.Add(quantityParameter);
                command.Parameters.Add(notesParameter);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Recupera una lista de movimientos de inventario, opcionalmente filtrados por identificador de producto.
        /// </summary>
        /// <param name="productId">Identificador del producto para filtrar los movimientos de inventario. Si es nulo, se recuperan todos los movimientos.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El valor de la tarea contiene una lista de movimientos de inventario.</returns>
        public async Task<List<InventoryMovementResponseDto>> GetMovementsAsync(long? productId = null)
        {
            var query = from movement in _context.InventoryMovements.AsNoTracking()
                        join product in _context.Products.AsNoTracking()
                            on movement.ProductId equals product.Id
                        select new InventoryMovementResponseDto
                        {
                            Id = movement.Id,
                            ProductId = movement.ProductId,
                            ProductCode = product.Code,
                            ProductName = product.Name,
                            MovementType = movement.MovementType,
                            Quantity = movement.Quantity,
                            Notes = movement.Notes,
                            CreatedAt = movement.CreatedAt
                        };

            if (productId.HasValue)
            {
                query = query.Where(movement => movement.ProductId == productId.Value);
            }

            return await query.OrderByDescending(movement => movement.CreatedAt).ToListAsync();
        }
    }
}
