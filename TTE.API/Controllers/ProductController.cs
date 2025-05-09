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

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var result = await _productService.GetProductById(productId);
            return result.Success ? Ok(result) : NotFound(result);
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
            var response = await _productService.DeleteProduct(productId, userRole);
            return Ok(response);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestProducts()
        {
            var result = await _productService.GetLatestProducts();
            return Ok(new GenericResponseDto<List<ProductResponseDto>>(true, "Latest products retrieved.", result));
        }

        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSellingProducts()
        {
            var result = await _productService.GetTopSellingProducts();
            return Ok(new GenericResponseDto<List<ProductResponseDto>>(true, "Top selling products retrieved.", result));
        }
    }
}