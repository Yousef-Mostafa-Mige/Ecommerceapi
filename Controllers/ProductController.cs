using Ecommerceapi.Dtos.ProductDtos;
using Ecommerceapi.services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerceapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductServices productService) : ControllerBase
    {
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
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productService.GetAllProductsAsync();
            if (products is null)
            {
                return BadRequest("Failed to retrieve products.");
            }
            return Ok(products);
        }
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
    }
}