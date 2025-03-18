using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
    }
}
