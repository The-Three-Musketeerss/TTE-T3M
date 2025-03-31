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
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _controller = new CartController(_mockCartService.Object);
        }

        private void SetUserId(int id)
        {
            var claims = new List<Claim> { new Claim("UserId", id.ToString()) };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task GetCart_ShouldReturnOk_WhenSuccessful()
        {
            SetUserId(1);

            var cartDto = new CartResponseDto
            {
                UserId = 1,
                ShoppingCart = new List<CartItemResponseDto>(),
                TotalBeforeDiscount = 100,
                TotalAfterDiscount = 90,
                ShippingCost = 10,
                FinalTotal = 100,
                CouponApplied = null
            };

            var response = new GenericResponseDto<CartResponseDto>(
                true,
                ValidationMessages.MESSAGE_CART_RETRIEVED_SUCCESSFULLY,
                cartDto
            );

            _mockCartService.Setup(s => s.GetCart(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetCart();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultData = Assert.IsType<GenericResponseDto<CartResponseDto>>(okResult.Value);

            Assert.Equal(1, ((CartResponseDto)resultData.Data).UserId);
            Assert.Equal(90, ((CartResponseDto)resultData.Data).TotalAfterDiscount);
        }




        [Fact]
        public async Task AddOrUpdateItem_ShouldReturnUnauthorized_WhenUserIdIsMissing()
        {
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var request = new CartItemRequestDto();

            // Act
            var result = await _controller.AddOrUpdateItem(request);

            // Assert
            var statusCode = (result as ObjectResult)?.StatusCode;
            Assert.Equal(StatusCodes.Status401Unauthorized, statusCode);
        }



        [Fact]
        public async Task ApplyCoupon_ShouldReturnOk_WhenCouponApplied()
        {
            SetUserId(5);
            var dto = new ApplyCouponDto { Code = "SAVE20" };
            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_APPLIED_SUCCESSFULLY);

            _mockCartService.Setup(s => s.ApplyCoupon(5, dto)).ReturnsAsync(response);

            // Act
            var result = await _controller.ApplyCoupon(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponseDto<string>>(ok.Value);
            Assert.True(data.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_APPLIED_SUCCESSFULLY, data.Message);
        }

        [Fact]
        public async Task RemoveItem_ShouldReturnOk_WhenItemDeleted()
        {
            SetUserId(2);
            var response = new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_CART_ITEM_DELETED);
            _mockCartService.Setup(s => s.RemoveCartItem(2, 1)).ReturnsAsync(response);

            // Act
            var result = await _controller.RemoveItem(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponseDto<string>>(ok.Value);
            Assert.True(data.Success);
        }
    }
}
