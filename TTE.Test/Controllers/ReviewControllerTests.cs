using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

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
            // Arrange
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
            // Arrange
            var productId = 1;
            var request = new ReviewRequestDto
            {
                User = "john",
                Review = "Great!",
                Rating = 5
            };

            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_REVIEW_ADDED_SUCCESSFULLY);

            _mockReviewService.Setup(s => s.AddReview(productId, request))
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
            // Arrange
            var productId = 1;
            var request = new ReviewRequestDto
            {
                User = "invalid_user",
                Review = "Not good",
                Rating = 2
            };

            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);

            _mockReviewService.Setup(s => s.AddReview(productId, request))
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
