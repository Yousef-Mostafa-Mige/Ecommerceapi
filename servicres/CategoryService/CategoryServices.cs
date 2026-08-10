using Ecommerceapi.Data;
using Ecommerceapi.Dtos.CategoryDtos;
using Ecommerceapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerceapi.services.CategoryService
{
    public class CategoryServices(AppDBContext context) : IcategoryServices
    {
        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryRequestDto)
        {
            if (categoryRequestDto is null)
            {
                return null!;
            }
            var category = new Category
            {
                Name = categoryRequestDto.Name
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return false;
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await context.Categories.ToListAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return null!;
            }
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name

            };
        }

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryRequestDto)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                return null!;
            }

            category.Name = categoryRequestDto.Name;

            await context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}