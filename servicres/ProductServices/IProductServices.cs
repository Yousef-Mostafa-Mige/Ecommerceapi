using Ecommerceapi.Dtos.ProductDtos;

namespace Ecommerceapi.services.ProductServices
{
    public interface IProductServices
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto);
        Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}