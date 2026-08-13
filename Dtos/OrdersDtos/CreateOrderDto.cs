using FluentValidation;

namespace Ecommerceapi.Dtos.OrderDtos;

public class CreateOrderDto
{
    public CreateOrderItemDto? Items { get; set; }

}
public class OrderValidationCreateOrderDto : AbstractValidator<CreateOrderDto>
{
    public OrderValidationCreateOrderDto()
    {
        RuleFor(x => x.Items).NotNull().WithMessage("Items are required.");
    }
}