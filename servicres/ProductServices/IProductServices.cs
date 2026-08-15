using Ecommerceapi.Dtos.Pagination;
using Ecommerceapi.Dtos.ProductDtos;
using Ecommerceapi.Dtos.search_sort_filiter;

namespace Ecommerceapi.services.ProductServices
{
    public interface IProductServices
    {
        Task<PaginatedResponseDto<ProductResponseDto>> GetAllProductsAsync(PaginatedRequestDto Page);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto);
        Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<PaginatedResponseDto<ProductResponseDto>>GetProductsAsync(ProductQueryRequest request);
    }
}