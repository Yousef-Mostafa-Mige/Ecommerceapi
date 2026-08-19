using Ecommerceapi.Data;
using Ecommerceapi.Dtos.CategoryDtos;
using Ecommerceapi.Dtos.Pagination;
using Ecommerceapi.Entities;
using ECommerceApi.Middleware;
using Microsoft.EntityFrameworkCore;

namespace Ecommerceapi.services.CategoryService
{
    public class CategoryServices(AppDBContext context) : IcategoryServices
    {
        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryRequestDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (categoryRequestDto is null)
                {
                    throw new BadRequestException("category should not be null");
                }
                var category = new Category
                {
                    Name = categoryRequestDto.Name
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var qury = context.Categories.Where(p => p.Id == id).AsQueryable();
            var category = await qury.ExecuteDeleteAsync();
            if (category == 0)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }

            // context.Categories.Remove(category);
            // await context.SaveChangesAsync();

            return true;
        }

        public async Task<PaginatedResponseDto<CategoryResponseDto>> GetAllCategoriesAsync(PaginatedRequestDto page)
        {
            var qury = context.Categories.AsQueryable().AsNoTracking();
            var categorycoute = await qury.CountAsync();
            var totlepages = (int)Math.Ceiling(categorycoute / (double)page.PageSize);
            if (page.PageNumber < 1 || page.PageNumber > totlepages)
            {
                throw new BadRequestException("http bad caregory");
            }
            var categories = await context.Categories.Skip((page.PageNumber - 1) * page.PageSize).Take(page.PageSize).ToListAsync();
            return new PaginatedResponseDto<CategoryResponseDto>
            {
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalCount = categorycoute,
                TotalPages = totlepages,
                Items = categories.Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name

            };
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryRequestDto)
        {
            var qury = context.Categories.Where(p => p.Id == id).AsQueryable();
            var category = await qury.ExecuteUpdateAsync(setters=>setters
            .SetProperty(p=>p.Name,p=>categoryRequestDto.Name??p.Name));
            if (category ==0)
            {
                throw new NotFoundException($"Category with ID {id} not found.");
            }
            return new CategoryResponseDto
            {
                Id = id,
                Name = categoryRequestDto.Name
            };
        }
    }
}