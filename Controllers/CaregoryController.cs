using Ecommerceapi.Dtos.CategoryDtos;
using Ecommerceapi.services.CategoryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerceapi.Dtos.Pagination;

namespace Ecommerceapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(IcategoryServices categoryService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            var result = await categoryService.CreateCategoryAsync(categoryDto);
            if (result is null)
            {
                return BadRequest("Failed to create category.");
            }
            return Ok(result);
        }
         [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await categoryService.GetCategoryByIdAsync(id);
            if (result is null)
            {
                return NotFound("Category not found.");
            }
            return Ok(result);
        }
         [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PaginatedRequestDto  page)
        {
            var result = await categoryService.GetAllCategoriesAsync(page);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
        {
            var result = await categoryService.UpdateCategoryAsync(id, categoryDto);
            if (result is null)
            {
                return NotFound("Category not found.");
            }
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound("Category not found.");
            }
            return Ok("Category deleted successfully.");
        }
    }
}