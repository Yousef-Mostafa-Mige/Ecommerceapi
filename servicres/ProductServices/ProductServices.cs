using Ecommerceapi.Data;
using Ecommerceapi.Dtos.Pagination;
using Ecommerceapi.Dtos.ProductDtos;
using Ecommerceapi.Dtos.search_sort_filiter;
using Ecommerceapi.Entities;
using ECommerceApi.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Ecommerceapi.services.ProductServices
{
    public class ProductServices(AppDBContext context) : IProductServices
    {
        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == productDto.CategoryId);


            if (category is null)
            {
                throw new NotFoundException($"Category with ID {productDto.CategoryId} not found.");
            }
            if (productDto.stok < 1)
            {
                throw new NotFoundException($"stok mast be over zero.");

            }
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                stok = productDto.stok,
                CategoryId = productDto.CategoryId
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                stock = product.stok,
                RowVersion = product.RowVersion,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await context.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
            if (product == 0)
            {
                throw new NotFoundException($"Product with ID {id} not found.");
            }
            return true;
        }

        public async Task<PaginatedResponseDto<ProductResponseDto>> GetAllProductsAsync(PaginatedRequestDto Page)
        {
            var qury = context.Products.Include(p => p.Category).AsQueryable().AsNoTracking();
            var totalProducts = await qury.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)Page.PageSize);
            if (Page.PageNumber < 1 || Page.PageNumber > totalPages)
            {
                throw new BadRequestException($"Page number {Page.PageNumber} is out of range. Total pages: {totalPages}.");
            }
            var products = await qury
            .Skip((Page.PageNumber - 1) * Page.PageSize)
            .Take(Page.PageSize)
            .ToListAsync();
            return new PaginatedResponseDto<ProductResponseDto>
            {
                PageNumber = Page.PageNumber,
                PageSize = Page.PageSize,
                TotalCount = totalProducts,
                TotalPages = totalPages,
                Items = products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    stock = p.stok,
                    RowVersion = p.RowVersion,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var qury = context.Products.Where(p => p.Id == id).Include(p => p.Category).AsQueryable().AsNoTracking();
            var product = await qury.FirstOrDefaultAsync();
            if (product is null)
            {
                throw new NotFoundException($"Product with ID {id} not found.");
            }

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                stock = product.stok,
                RowVersion = product.RowVersion,

                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(
    int id,
    UpdateProductDto productDto)
        {
            var category = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == productDto.CategoryId);

            if (category is null)
            {
                throw new NotFoundException(
                    $"Category with ID {productDto.CategoryId} not found.");
            }

            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                throw new NotFoundException(
                    $"Product with ID {id} not found.");
            }

            product.Name = productDto.Name ?? product.Name;
            product.Price = productDto.Price ?? product.Price;
            product.stok = productDto.stok ?? product.stok;
            product.CategoryId = productDto.CategoryId;

            context.Entry(product)
                .Property(p => p.RowVersion)
                .OriginalValue = productDto.RowVersion;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException(
                    "Product was modified by another user.");
            }

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                stock = product.stok,
                RowVersion = product.RowVersion,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                CreatedAt = product.CreatedAt
            };
        }
        public async Task<PaginatedResponseDto<ProductResponseDto>> GetProductsAsync(ProductQueryRequest request)
        {
            var query = context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            // =========================
            // SEARCH
            // =========================

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(request.Search));
            }


            // =========================
            // FILTER
            // =========================

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == request.CategoryId.Value);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= request.MaxPrice.Value);
            }


            // =========================
            // SORT
            // =========================

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "price":

                        if (request.SortOrder?.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(p => p.Price);
                        }
                        else
                        {
                            query = query.OrderBy(p => p.Price);
                        }

                        break;


                    case "name":

                        if (request.SortOrder?.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(p => p.Name);
                        }
                        else
                        {
                            query = query.OrderBy(p => p.Name);
                        }

                        break;


                    case "date":

                        if (request.SortOrder?.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(p => p.CreatedAt);
                        }
                        else
                        {
                            query = query.OrderBy(p => p.CreatedAt);
                        }

                        break;


                    case "id":

                        if (request.SortOrder?.ToLower() == "desc")
                        {
                            query = query.OrderByDescending(p => p.Id);
                        }
                        else
                        {
                            query = query.OrderBy(p => p.Id);
                        }

                        break;


                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }


            // =========================
            // COUNT
            // =========================

            var totalCount = await query.CountAsync();


            // =========================
            // PAGINATION
            // =========================

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)request.Page.PageSize);


            if (request.Page.PageNumber < 1 ||
                (totalPages > 0 &&
                 request.Page.PageNumber > totalPages))
            {
                throw new BadRequestException(
                    $"Page {request.Page.PageNumber} not found.");
            }


            var products = await query
                .Skip(
                    (request.Page.PageNumber - 1)
                    * request.Page.PageSize)
                .Take(request.Page.PageSize)
                .ToListAsync();


            // =========================
            // RESPONSE
            // =========================

            return new PaginatedResponseDto<ProductResponseDto>
            {
                PageNumber = request.Page.PageNumber,
                PageSize = request.Page.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,

                Items = products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    stock = p.stok,
                    RowVersion = p.RowVersion,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };
        }

    }
}
