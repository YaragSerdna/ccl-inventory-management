namespace Inventory.Api.Models
{
    public class InventoryMovement
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public string MovementType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Product? Product { get; set; }
    }
}
