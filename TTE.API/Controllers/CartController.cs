using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.API.Controllers
{
    [Authorize(Policy = "ShopperOnly")]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _cartService.GetCart(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateItem([FromBody] CartItemRequestDto request)
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _cartService.AddOrUpdateCartItem(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _cartService.RemoveCartItem(userId, productId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto request)
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _cartService.ApplyCoupon(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
