using Ecommerceapi.Dtos.OrderDtos;

namespace Ecommerceapi.services.OrderServices
{
    public interface IOrderServices
    {
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId);
        Task<OrderResponseDto> CreateOrderAsync(int userId,CreateOrderItemDto orderDto);
        Task<bool> DeleteOrderAsync(int id, int userId);
        Task<List<OrderResponseDto>?> GetOrdersByUserIdAsync(int id);
    }
}