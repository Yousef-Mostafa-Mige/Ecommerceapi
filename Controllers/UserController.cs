using Ecommerceapi.Dtos.UserDtos;
using Ecommerceapi.services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerceapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserServices userService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await userService.GetUserByIdAsync(id);
            if (result is null)
            {
                return NotFound("User not found.");
            }
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            var result = await userService.RegisterUserAsync(registerUserDto);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            var result = await userService.LoginUserAsync(loginUserDto);
            if (result is null)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(result);
        }
    }
}