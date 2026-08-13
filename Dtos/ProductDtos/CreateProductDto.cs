using Ecommerceapi.Dtos.ProductDtos;
using FluentValidation;

namespace Ecommerceapi.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
    }
}
public class ProductValidationCreateProductDto : AbstractValidator<CreateProductDto>
    {
        public ProductValidationCreateProductDto()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than zero.");
        }
    }