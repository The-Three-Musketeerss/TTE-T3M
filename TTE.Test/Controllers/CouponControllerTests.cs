using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;

namespace TTE.Tests.Controllers
{
    public class CouponControllerTests
    {
        private readonly Mock<ICouponService> _mockCouponService;
        private readonly CouponController _controller;

        public CouponControllerTests()
        {
            _mockCouponService = new Mock<ICouponService>();
            _controller = new CouponController(_mockCouponService.Object);
        }

        [Fact]
        public async Task CreateCoupon_ShouldReturnOk_WhenSuccess()
        {
            var request = new CouponRequestDto { Code = "SAVE10", Discount = 10 };
            var response = new GenericResponseDto<string>(true, "Coupon created successfully.");
            _mockCouponService.Setup(s => s.CreateCoupon(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCoupon(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(value.Success);
        }

        [Fact]
        public async Task CreateCoupon_ShouldReturnBadRequest_WhenFail()
        {
            var request = new CouponRequestDto { Code = "SAVE10", Discount = 10 };
            var response = new GenericResponseDto<string>(false, "Coupon code already exists.");
            _mockCouponService.Setup(s => s.CreateCoupon(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCoupon(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(badResult.Value);
            Assert.False(value.Success);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnOk_WhenSuccess()
        {
            var response = new GenericResponseDto<string>(true, "Coupon updated successfully.");
            _mockCouponService.Setup(s => s.UpdateCoupon(1, It.IsAny<CouponRequestDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCoupon(1, new CouponRequestDto());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(((GenericResponseDto<string>)okResult.Value).Success);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            var response = new GenericResponseDto<string>(false, "Coupon not found.");
            _mockCouponService.Setup(s => s.UpdateCoupon(1, It.IsAny<CouponRequestDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCoupon(1, new CouponRequestDto());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.False(((GenericResponseDto<string>)notFoundResult.Value).Success);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnOk_WhenSuccess()
        {
            var response = new GenericResponseDto<string>(true, "Coupon deleted successfully.");
            _mockCouponService.Setup(s => s.DeleteCoupon(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCoupon(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(((GenericResponseDto<string>)okResult.Value).Success);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnNotFound_WhenFail()
        {
            var response = new GenericResponseDto<string>(false, "Coupon not found.");
            _mockCouponService.Setup(s => s.DeleteCoupon(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCoupon(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.False(((GenericResponseDto<string>)notFoundResult.Value).Success);
        }
    }
}
