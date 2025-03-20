using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Application.DTOs;

namespace TTE.API.Controllers
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
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        
        [HttpPut("{username}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _userService.UpdateUser(username, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUsers([FromBody] List<string> usernames)
        {
            var result = await _userService.DeleteUsers(usernames);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
