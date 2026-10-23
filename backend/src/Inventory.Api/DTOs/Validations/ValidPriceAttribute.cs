using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.DTOs.Validations
{
    public sealed class ValidPriceAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determina si el valor especificado es un precio decimal válido con hasta dos decimales en el rango de 0 a 9.999.999.999,99.
        /// </summary>
        /// <param name="value">Valor a validar.</param>
        /// <returns>true si el valor es un precio decimal válido; de lo contrario, false.</returns>
        public override bool IsValid(object? value)
        {
            if (value is not decimal price)
                return false;

            return price >= 0 && price <= 9999999999.99m && decimal.Round(price, 2) == price;
        }
    }
}
