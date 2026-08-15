using Ecommerceapi.Dtos.Pagination;
using Ecommerceapi.Dtos.ProductDtos;
using Ecommerceapi.Dtos.search_sort_filiter;
using Ecommerceapi.services.ProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerceapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductServices productService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginatedRequestDto page)
        {
            var products = await productService.GetAllProductsAsync(page);

            return Ok(products);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var product = await productService.CreateProductAsync(productDto);
            if (product is null)
            {
                return BadRequest("Failed to create product.");
            }
            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            var product = await productService.UpdateProductAsync(id, productDto);
            if (product is null)
            {
                return NotFound("Product not found.");
            }
            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound("Product not found.");
            }
            return Ok("Product deleted successfully.");
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryRequest request)
        {
            var result = await productService.GetProductsAsync(request);

            return Ok(result);
        }
    }
}