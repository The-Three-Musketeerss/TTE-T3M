﻿using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons;

namespace TTE.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (response == null)
                return Unauthorized(new { message = SystemConstants.MESSAGE_LOGIN_FAIL });

            return Ok(response);
        }
    }
}
