namespace Inventory.Api.DTOs.Inventory
{
    public class CurrentInventoryDto
    {
        public long ProductId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Stock { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal InventoryValue { get; set; }
    }
}
