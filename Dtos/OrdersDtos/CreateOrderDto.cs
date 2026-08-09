namespace Ecommerceapi.Dtos.OrderDtos;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; }
        = new List<CreateOrderItemDto>();
}