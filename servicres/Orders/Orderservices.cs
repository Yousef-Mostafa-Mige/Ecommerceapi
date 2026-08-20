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
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (orderDto is null)
                {
                    throw new BadRequestException(nameof(orderDto));
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
                    throw new NotFoundException(
                        $"Product with ID {orderDto.ProductId} not found.");
                }

                if (orderDto.Quantity <= 0)
                {
                    throw new BadRequestException("Quantity must be greater than zero.");
                }

                if (orderDto.Quantity > product.stok)
                {
                    throw new BadRequestException(
                        "Not enough stock available.");
                }
                product.stok -= orderDto.Quantity;
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
                var response = new OrderResponseDto
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
                await transaction.CommitAsync();
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteOrderAsync(int id, int userId)
        {
            var q = context.Orders.Where(p => p.Id == id&& p.UserId==userId).AsQueryable();
            var order = await q.ExecuteDeleteAsync();
            // var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == 0)
            {
                throw new NotFoundException($"order with ID {id} not found."); ;
            }
            // context.Orders.Remove(order);
            // await context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResponseDto<OrderResponseDto>> GetAllOrdersAsync(PaginatedRequestDto Page)
        {
            var query = context.Orders.AsQueryable().AsNoTracking();
            var totleorders = await query.CountAsync();
            var totlepages = (int)Math.Ceiling(totleorders / (double)Page.PageSize);
            if (Page.PageNumber < 1 || Page.PageNumber > totlepages)
            {
                throw new BadRequestException("error in pages");
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
        public async Task<PaginatedResponseDto<OrderResponseDto>?> GetOrdersByUserIdAsync(int id, PaginatedRequestDto page)
        {
            var query = context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == id);
            var totalOrders = await query.CountAsync();

            if (totalOrders == 0)
            {
                return new PaginatedResponseDto<OrderResponseDto>
                {
                    PageNumber = page.PageNumber,
                    PageSize = page.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = new List<OrderResponseDto>()
                };
            }
            var totalPages = (int)Math.Ceiling(totalOrders / (double)page.PageSize);

            if (page.PageNumber < 1 || page.PageNumber > totalPages)
            {
                throw new BadRequestException("Invalid page number.");
            }


            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page.PageNumber - 1) * page.PageSize)
                .Take(page.PageSize)
                .Select(order => new OrderResponseDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(item => new OrderItemResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product != null ? item.Product.Name : string.Empty,
                        Quantity = item.Quantity,
                        Price = item.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return new PaginatedResponseDto<OrderResponseDto>
            {
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalCount = totalOrders,
                TotalPages = totalPages,
                Items = orders
            };
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, int userId)
        {
            var order = await context.Orders.Where(o => o.Id == id && o.UserId == userId)
                .AsNoTracking()
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