using Inventory.Api.DTOs.Validations;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [ValidPrice(ErrorMessage = "El precio debe ser positivo y tener máximo dos decimales.")]
        public decimal UnitPrice { get; set; }
    }
}
