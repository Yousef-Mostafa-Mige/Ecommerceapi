using Ecommerceapi.Data;
using Ecommerceapi.Dtos.OrderDtos;
using Ecommerceapi.Dtos.Pagination;
using Ecommerceapi.Entities;
using ECommerceApi.Middleware;
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
                throw new ArgumentNullException(nameof(orderDto));
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            var order = new Order
            {
                UserId = userId
            };


            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == orderDto.ProductId);

            if (product is null)
            {
                throw new NotFoundException($"Product with ID {orderDto.ProductId} not found.");
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
                throw new NotFoundException($"Order with ID {order.Id} not found.");
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

        public async Task<PaginatedResponseDto<OrderResponseDto>> GetAllOrdersAsync(PaginatedRequestDto Page)
        {
            var query = context.Orders.AsQueryable();
            var totleorders = await query.CountAsync();
            var totlepages = (int)Math.Ceiling(totleorders / (double)Page.PageSize);
            if (Page.PageNumber < 1 || Page.PageNumber > totlepages)
            {
                throw new BadHttpRequestException("error in pages");
            }
            var orders = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Skip((Page.PageNumber - 1) * Page.PageSize)
                .Take(Page.PageSize)
                .ToListAsync();
            if (orders is null)
            {
                throw new NotFoundException("No orders found.");
            }
            return new PaginatedResponseDto<OrderResponseDto>
            {
                PageNumber = Page.PageNumber,
                PageSize = Page.PageSize,
                TotalCount = totleorders,
                TotalPages = totlepages,
                Items = orders.Select(order => new OrderResponseDto
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
                }).ToList()
            };

        }
        public async Task<PaginatedResponseDto<OrderResponseDto>?> GetOrdersByUserIdAsync(int id, PaginatedRequestDto Page)
        {
            var qury = context.Orders.AsQueryable();
            var totleorders = await qury.CountAsync();
            var totlepage = (int)Math.Ceiling(totleorders / (double)Page.PageSize);
            if (Page.PageNumber < 1 || Page.PageNumber > totlepage)
            {
                throw new BadHttpRequestException("http error orders");
            }
            var orders = await context.Orders
                .Where(o => o.UserId == id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Skip((Page.PageNumber - 1) * Page.PageSize)
                .Take(Page.PageSize)
                .ToListAsync();
            return new PaginatedResponseDto<OrderResponseDto>
            {
                PageNumber = Page.PageNumber,
                PageSize = Page.PageSize,
                TotalCount = totleorders,
                TotalPages = totlepage,
                Items = orders.Select(order => new OrderResponseDto
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
                }).ToList()
            };
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId)
        {
            var order = await context.Orders.Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
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