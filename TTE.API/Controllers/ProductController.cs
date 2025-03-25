using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != AppConstants.ADMIN && userRole != AppConstants.EMPLOYEE)
            {
                return Forbid();
            }

            var result = await _productService.CreateProducts(request, userRole);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? category,
            [FromQuery] string? orderBy,
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _productService.GetProducts(category, orderBy, descending, page, pageSize);
            return Ok(response);
        }

        [Authorize(Policy = "CanAccessDashboard")]
        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductUpdateRequestDto request)
        {
            var response = await _productService.UpdateProduct(productId, request);
            return Ok(response);
        }

        [Authorize(Policy = "CanAccessDashboard")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized(new { message = ValidationMessages.MESSAGE_ROLE_NOT_FOUND });
            }
            var response = await _productService.DeleteProduct(productId);
            return Ok(response);
        }
    }
}