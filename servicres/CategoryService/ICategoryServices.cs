using Ecommerceapi.Dtos.CategoryDtos;
using Ecommerceapi.Dtos.Pagination;

namespace Ecommerceapi.services.CategoryService
{
   public interface IcategoryServices
    {
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<PaginatedResponseDto<CategoryResponseDto>> GetAllCategoriesAsync(PaginatedRequestDto page);
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto categoryRequestDto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryRequestDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}