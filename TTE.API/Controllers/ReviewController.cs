using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;

namespace TTE.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var result = await _reviewService.GetReviews(productId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, [FromBody] ReviewRequestDto request)
        {
            var result = await _reviewService.AddReview(productId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
