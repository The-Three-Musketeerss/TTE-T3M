using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

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
            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_CREATED_SUCCESSFULLY);
            _mockCouponService.Setup(s => s.CreateCoupon(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCoupon(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_CREATED_SUCCESSFULLY, value.Message);
        }

        [Fact]
        public async Task CreateCoupon_ShouldReturnBadRequest_WhenFail()
        {
            var request = new CouponRequestDto { Code = "SAVE10", Discount = 10 };
            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_CODE_ALREADY_EXISTS);
            _mockCouponService.Setup(s => s.CreateCoupon(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCoupon(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(badResult.Value);
            Assert.False(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_CODE_ALREADY_EXISTS, value.Message);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnOk_WhenSuccess()
        {
            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_UPDATED_SUCCESSFULLY);
            _mockCouponService.Setup(s => s.UpdateCoupon(1, It.IsAny<CouponRequestDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCoupon(1, new CouponRequestDto());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_UPDATED_SUCCESSFULLY, value.Message);
        }

        [Fact]
        public async Task UpdateCoupon_ShouldReturnNotFound_WhenCouponDoesNotExist()
        {
            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_NOT_FOUND);
            _mockCouponService.Setup(s => s.UpdateCoupon(1, It.IsAny<CouponRequestDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCoupon(1, new CouponRequestDto());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(notFoundResult.Value);
            Assert.False(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_NOT_FOUND, value.Message);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnOk_WhenSuccess()
        {
            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_DELETED_SUCCESSFULLY);
            _mockCouponService.Setup(s => s.DeleteCoupon(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCoupon(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_DELETED_SUCCESSFULLY, value.Message);
        }

        [Fact]
        public async Task DeleteCoupon_ShouldReturnNotFound_WhenFail()
        {
            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_NOT_FOUND);
            _mockCouponService.Setup(s => s.DeleteCoupon(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteCoupon(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = Assert.IsType<GenericResponseDto<string>>(notFoundResult.Value);
            Assert.False(value.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_NOT_FOUND, value.Message);
        }
    }
}
