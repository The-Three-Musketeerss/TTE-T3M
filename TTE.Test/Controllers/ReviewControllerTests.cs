using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using Xunit;

namespace TTE.Tests.Controllers
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly ReviewController _controller;

        public ReviewControllerTests()
        {
            _mockReviewService = new Mock<IReviewService>();
            _controller = new ReviewController(_mockReviewService.Object);
        }

        private void SetUserWithId(int userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("UserId", userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetReviews_ShouldReturnOk_WhenProductHasReviews()
        {
            // Arrange
            var productId = 1;
            var reviews = new List<ReviewResponseDto>
            {
                new ReviewResponseDto { User = "alice", Review = "Good!", Rating = 4 }
            };

            var response = new GenericResponseDto<List<ReviewResponseDto>>(true, "", reviews);

            _mockReviewService.Setup(s => s.GetReviews(productId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetReviews(productId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task GetReviews_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            var productId = 999;
            var response = new GenericResponseDto<List<ReviewResponseDto>>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);

            _mockReviewService.Setup(s => s.GetReviews(productId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetReviews(productId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task AddReview_ShouldReturnOk_WhenReviewIsCreated()
        {
            var productId = 1;
            var userId = 10;

            SetUserWithId(userId);

            var request = new ReviewRequestDto
            {
                Review = "Great!",
                Rating = 5
            };

            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_REVIEW_ADDED_SUCCESSFULLY);

            _mockReviewService.Setup(s => s.AddReview(productId, request, userId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AddReview(productId, request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task AddReview_ShouldReturnBadRequest_WhenFailedToCreate()
        {
            var productId = 1;
            var userId = 10;

            SetUserWithId(userId);

            var request = new ReviewRequestDto
            {
                Review = "Not good",
                Rating = 2
            };

            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);

            _mockReviewService.Setup(s => s.AddReview(productId, request, userId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AddReview(productId, request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }
    }
}
