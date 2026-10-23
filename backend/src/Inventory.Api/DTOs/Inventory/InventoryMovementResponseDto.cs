namespace Inventory.Api.DTOs.Inventory
{
    public class InventoryMovementResponseDto
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string MovementType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
