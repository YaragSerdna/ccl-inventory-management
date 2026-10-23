using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.DTOs.Inventory
{
    public class RegisterInventoryMovementDto
    {
        [Range(1, long.MaxValue)]
        public long ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
