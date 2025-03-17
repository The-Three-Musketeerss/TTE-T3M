using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Infrastructure.Models;

namespace TTE.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] GenericRequestDto<ShopperRequestDto> request)
        {
            var result = await _userService.RegisterUser(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
