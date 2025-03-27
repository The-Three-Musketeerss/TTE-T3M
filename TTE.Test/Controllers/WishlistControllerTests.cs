using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.Tests.Controllers
{
    public class WishlistControllerTests
    {
        private readonly Mock<IWishlistService> _mockWishlistService;
        private readonly WishlistController _controller;

        public WishlistControllerTests()
        {
            _mockWishlistService = new Mock<IWishlistService>();
            _controller = new WishlistController(_mockWishlistService.Object);
        }

        private void SetUserIdClaim(string? userId)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim("UserId", userId));
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetWishlist_ShouldReturnOk_WhenUserIdIsPresent()
        {
            SetUserIdClaim("123");

            var expectedDto = new WishlistResponseDto { Wishlist = new List<int> { 1, 2, 3 } };
            var expectedResponse = new GenericResponseDto<WishlistResponseDto>(
                true,
                ValidationMessages.MESSAGE_WISHLIST_RETRIEVED_SUCCESSFULLY,
                expectedDto
            );

            _mockWishlistService
                .Setup(s => s.GetWishlist(123))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetWishlist();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<WishlistResponseDto>>(okResult.Value);
            var wishlistData = Assert.IsAssignableFrom<WishlistResponseDto>(response.Data);

            Assert.True(response.Success);
            Assert.Equal(expectedDto.Wishlist, wishlistData.Wishlist);
        }

        [Fact]
        public async Task GetWishlist_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUserIdClaim(null);

            // Act
            var result = await _controller.GetWishlist();

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);
        }

        [Fact]
        public async Task AddToWishlist_ShouldReturnOk_WhenUserIdIsPresent()
        {
            SetUserIdClaim("123");
            int productId = 5;

            var expectedResponse = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_WISHLIST_PRODUCT_ADDED);

            _mockWishlistService
                .Setup(s => s.AddToWishlist(123, productId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AddToWishlist(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.MESSAGE_WISHLIST_PRODUCT_ADDED, response.Message);
        }

        [Fact]
        public async Task AddToWishlist_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUserIdClaim(null);
            int productId = 5;

            // Act
            var result = await _controller.AddToWishlist(productId);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);
        }

        [Fact]
        public async Task RemoveFromWishlist_ShouldReturnOk_WhenUserIdIsPresent()
        {
            SetUserIdClaim("123");
            int productId = 5;

            var expectedResponse = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_WISHLIST_PRODUCT_REMOVED);

            _mockWishlistService
                .Setup(s => s.RemoveFromWishlist(123, productId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.RemoveFromWishlist(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.MESSAGE_WISHLIST_PRODUCT_REMOVED, response.Message);
        }

        [Fact]
        public async Task RemoveFromWishlist_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            SetUserIdClaim(null);
            int productId = 5;

            // Act
            var result = await _controller.RemoveFromWishlist(productId);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);
        }
    }
}
