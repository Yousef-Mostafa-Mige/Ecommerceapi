using System.Security.Claims;
using Ecommerceapi.Dtos.OrderDtos;
using Ecommerceapi.services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerceapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderServices orderService) : ControllerBase
    {
       

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderItemDto orderDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Unauthorized("User ID claim not found.");
            }
            var userId = int.Parse(userIdClaim);
            var result = await orderService.CreateOrderAsync(userId, orderDto);
            if (result is null)
            {
                return BadRequest("Failed to create order.");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Unauthorized("User ID claim not found.");
            }
            var result = await orderService.GetOrderByIdAsync(id, int.Parse(userIdClaim));
            if (result is null)
            {
                return NotFound("Order not found.");
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
           
            var result = await orderService.GetAllOrdersAsync();
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Unauthorized("User ID claim not found.");
            }
            var result = await orderService.DeleteOrderAsync(id, int.Parse(userIdClaim));
            if (!result)
            {
                return NotFound("Order not found.");
            }
            return Ok("Order deleted successfully.");
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Unauthorized("User ID claim not found.");
            }
            var result = await orderService.GetOrdersByUserIdAsync(int.Parse(userIdClaim));
            return Ok(result);
        }
    }
}