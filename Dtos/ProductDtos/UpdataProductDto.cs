using System.ComponentModel.DataAnnotations;
using Ecommerceapi.Dtos.ProductDtos;
using FluentValidation;

namespace Ecommerceapi.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } = string.Empty;
        public decimal ?Price { get; set; }
        public int? stok { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public long RowVersion { get; set; } 
    }
}
