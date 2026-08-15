using Ecommerceapi.Dtos.OrderDtos;
using Ecommerceapi.Dtos.Pagination;

namespace Ecommerceapi.services.OrderServices
{
    public interface IOrderServices
    {
        Task<PaginatedResponseDto<OrderResponseDto>> GetAllOrdersAsync(PaginatedRequestDto Page);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId);
        Task<OrderResponseDto> CreateOrderAsync(int userId,CreateOrderItemDto orderDto);
        Task<bool> DeleteOrderAsync(int id, int userId);
        Task<PaginatedResponseDto<OrderResponseDto>?> GetOrdersByUserIdAsync(int id,PaginatedRequestDto Page);
    }
}