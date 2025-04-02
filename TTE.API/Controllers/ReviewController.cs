using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Application.Services;
using TTE.Commons.Constants;

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

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
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
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new { message = ValidationMessages.MESSAGE_USER_NOT_FOUND });

            var result = await _reviewService.AddReview(productId, request, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
