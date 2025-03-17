using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Application.Services;
using TTE.Commons.Constants;

namespace TTE.API.Controllers
{
    [Route(AppConstants.API_AUTH)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(AppConstants.SIGN_UP)]
        public async Task<IActionResult> RegisterUser([FromBody] ShopperRequestDto request)
        {
            var result = await _authService.RegisterUser(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost(AppConstants.LOG_IN)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);

            if (response == null)
                return Unauthorized(new { message = AuthenticationMessages.MESSAGE_LOGIN_FAIL });

            return Ok(response);
        }


    }
}
