using Ecommerceapi.Data;
using Ecommerceapi.Dtos.OrderDtos;
using Ecommerceapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerceapi.services.OrderServices
{
    public class OrderServices(AppDBContext context) : IOrderServices
    {
        public async Task<OrderResponseDto> CreateOrderAsync(int userId,
    CreateOrderItemDto orderDto)
        {
            if (orderDto is null)
            {
                return null!;
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return null!;
            }

            var order = new Order
            {
                UserId = userId
            };

            
                var product = await context.Products
                    .FirstOrDefaultAsync(p => p.Id == orderDto.ProductId);

                if (product is null)
                {
                    return null!;
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = orderDto.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);
            

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var createdOrder = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (createdOrder is null)
            {
                return null!;
            }
            return new OrderResponseDto
            {
                Id = createdOrder.Id,
                UserId = createdOrder.UserId,
                CreatedAt = createdOrder.CreatedAt,

                Items = createdOrder.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                }).ToList()
            };
        }
        public async Task<bool> DeleteOrderAsync(int id, int userId)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order is null)
            {
                return false;
            }
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
            if (orders is null)
            {
                return null!;
            }
            return new List<OrderResponseDto>(orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                }).ToList()
            }));
        }
        public async Task<List<OrderResponseDto>?> GetOrdersByUserIdAsync(int id)
        {
            var orders = await context.Orders
                .Where(o => o.UserId == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                }).ToList()

            }).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId)
        {
            var order = await context.Orders.Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();
            if (order is null)
            {
                return null!;
            }
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                }).ToList()
            };
        }
    }
}