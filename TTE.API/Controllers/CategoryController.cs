using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using System.Security.Claims;

namespace TTE.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "CanAccessDashboard")]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new {message = ValidationMessages.MESSAGE_ROL_NOT_FOUND});
            }

            var response = await _categoryService.DeleteCategory(categoryId, userRole);

            return response.Success ? Ok(response) : BadRequest(response);
        }

    }
}