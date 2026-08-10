using Ecommerceapi.Dtos.CategoryDtos;

namespace Ecommerceapi.services.CategoryService
{
   public interface IcategoryServices
    {
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryRequestDto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryRequestDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}