using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;

namespace TTE.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
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
    }
}