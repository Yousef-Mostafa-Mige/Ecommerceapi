namespace Ecommerceapi.Dtos.OrderDtos;

public class OrderResponseDto
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderItemResponseDto> Items { get; set; }
        = new List<OrderItemResponseDto>();
}