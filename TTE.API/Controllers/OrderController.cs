﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.API.Controllers
{
    [Authorize(Policy = "ShopperOnly")]
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _orderService.CreateOrderFromCart(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _orderService.GetOrdersByUser(userId);
            return Ok(result);
        }
    }
}
