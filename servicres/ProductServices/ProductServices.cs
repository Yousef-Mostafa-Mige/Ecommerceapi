using Ecommerceapi.Data;
using Ecommerceapi.Dtos.ProductDtos;
using Ecommerceapi.Entities;
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
                return null!;
            }
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product =  await context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
            {
                return false;
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await context.Products.Include(p => p.Category).ToListAsync();
            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await context.Products.Where(p => p.Id == id).Include(p => p.Category).FirstOrDefaultAsync();
            if (product is null)
            {
                return null!;
            }

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
            {
                return null!;
            }
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == productDto.CategoryId);
             if (category is null)
            {
                return null!;
            }

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;

            await context.SaveChangesAsync();


            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                CreatedAt = product.CreatedAt
            };
        }
        }
    }
