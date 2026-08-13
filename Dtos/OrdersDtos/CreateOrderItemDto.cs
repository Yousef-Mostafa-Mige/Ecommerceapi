using FluentValidation;

namespace Ecommerceapi.Dtos.OrderDtos;

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }

}
public class OrderItemValidationCreateOrderItemDto : AbstractValidator<CreateOrderItemDto>
    {
        public OrderItemValidationCreateOrderItemDto()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId must be greater than zero.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }