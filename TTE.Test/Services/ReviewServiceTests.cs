using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;
using System;
using System.Linq.Expressions;

namespace TTE.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IGenericRepository<Review>> _mockReviewRepo = new();
        private readonly Mock<IGenericRepository<Rating>> _mockRatingRepo = new();
        private readonly Mock<IRatingRepository> _mockRatingCustomRepo = new();
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo = new();
        private readonly Mock<IGenericRepository<User>> _mockUserRepo = new();
        private readonly Mock<IGenericRepository<Job>> _mockJobRepo = new();

        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _service = new ReviewService(
                _mockReviewRepo.Object,
                _mockRatingRepo.Object,
                _mockProductRepo.Object,
                _mockUserRepo.Object,
                _mockRatingCustomRepo.Object,
                _mockJobRepo.Object
            );
        }

        [Fact]
        public async Task AddReview_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var productId = 1;
            var userId = 10;
            var user = new User { Id = userId, UserName = "john_doe", Name = "John" };
            var product = new Product { Id = productId };

            var request = new ReviewRequestDto
            {
                Rating = 5,
                Review = "Great product!"
            };

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                .ReturnsAsync(product);

            _mockUserRepo.Setup(r => r.GetByCondition(u => u.Id == userId))
                .ReturnsAsync(user);

            _mockReviewRepo.Setup(r => r.GetByCondition(r => r.ProductId == productId && r.UserId == user.Id))
                .ReturnsAsync((Review)null);

            _mockRatingRepo.Setup(r => r.GetByCondition(r => r.ProductId == productId && r.UserId == user.Id))
                .ReturnsAsync((Rating)null);

            _mockReviewRepo.Setup(r => r.Add(It.IsAny<Review>()))
                .ReturnsAsync(1);

            _mockRatingRepo.Setup(r => r.Add(It.IsAny<Rating>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddReview(productId, request, userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_REVIEW_ADDED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task AddReview_ShouldFail_WhenProductNotFound()
        {
            var userId = 1;
            _mockProductRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            var result = await _service.AddReview(1, new ReviewRequestDto(), userId);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task AddReview_ShouldFail_WhenUserNotFound()
        {
            var productId = 1;
            var userId = 999;

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                .ReturnsAsync(new Product { Id = productId });

            _mockUserRepo.Setup(r => r.GetByCondition(u => u.Id == userId))
                .ReturnsAsync((User)null);

            var result = await _service.AddReview(productId, new ReviewRequestDto
            {
                Review = "Nice",
                Rating = 4
            }, userId);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_USER_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task GetReviews_ShouldReturnReviews_WhenProductExists()
        {
            var productId = 1;
            var userId = 2;

            var product = new Product { Id = productId };
            var user = new User { Id = userId, Name = "Alice" };

            var reviews = new List<Review>
            {
                new Review { ProductId = productId, UserId = userId, Comment = "Nice!", User = user }
            };

            var ratings = new List<Rating>
            {
                new Rating { ProductId = productId, UserId = userId, Rate = 4 }
            };

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                .ReturnsAsync(product);

            _mockReviewRepo.Setup(r => r.GetAllByCondition(r => r.ProductId == productId, AppConstants.USER))
                .ReturnsAsync(reviews);

            _mockRatingCustomRepo.Setup(r => r.GetRatingsByProductId(productId))
                .ReturnsAsync(ratings);

            var result = await _service.GetReviews(productId);

            var data = result.Data as List<ReviewResponseDto>;

            Assert.True(result.Success);
            Assert.Single(data);
            Assert.Equal("Alice", data[0].User);
            Assert.Equal(4, data[0].Rating);
        }

        [Fact]
        public async Task GetReviews_ShouldFail_WhenProductDoesNotExist()
        {
            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == 999))
                .ReturnsAsync((Product)null);

            var result = await _service.GetReviews(999);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }
    }
}
