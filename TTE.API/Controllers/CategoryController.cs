using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Application.DTOs;
using TTE.Application.Services;
using System.Security.Claims;
using TTE.Commons.Constants;

namespace TTE.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Policy = "CanAccessDashboard")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDto request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = ValidationMessages.MESSAGE_ROLE_NOT_FOUND });
            }
            if (userRole != AppConstants.ADMIN && userRole != AppConstants.EMPLOYEE)
            {
                return Forbid();
            }
            var result = await _categoryService.CreateCategory(request, userRole);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _categoryService.GetCategories();
            return Ok(response);
        }
    
        [Authorize(Policy = "CanAccessDashboard")]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new {message = ValidationMessages.MESSAGE_ROLE_NOT_FOUND});
            }

            var response = await _categoryService.DeleteCategory(categoryId, userRole);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize(Policy = "CanAccessDashboard")]
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId,[FromBody] CategoryRequestDto request)
        {
            request.Id = categoryId;
            var response = await _categoryService.UpdateCategory(categoryId, request);

            return response.Success ? Ok(response) : BadRequest(response);
        }

    }
}