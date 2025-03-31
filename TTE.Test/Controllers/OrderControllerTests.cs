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
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrderController(_mockOrderService.Object);
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
        public async Task CreateOrder_ShouldReturnUnauthorized_WhenNoUserId()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            // Act
            var result = await _controller.CreateOrder();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WhenSuccessful()
        {
            SetUserId(1);
            var response = new GenericResponseDto<int>(true, "ok", 123);

            _mockOrderService.Setup(s => s.CreateOrderFromCart(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateOrder();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<GenericResponseDto<int>>(ok.Value);
            Assert.True(data.Success);
            Assert.Equal(123, data.Data);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnList()
        {
            SetUserId(2);
            var orders = new List<OrderDto>
            {
                new OrderDto { Id = 1, UserId = 2, TotalBeforeDiscount = 100, TotalAfterDiscount = 80 }
            };

            var response = new GenericResponseDto<List<OrderDto>>(true, "ok", orders);
            _mockOrderService.Setup(s => s.GetOrdersByUser(2)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<GenericResponseDto<List<OrderDto>>>(ok.Value);

            var ordersList = Assert.IsType<List<OrderDto>>(resultDto.Data);

            Assert.Single(ordersList);
            Assert.Equal(2, ordersList.First().UserId);
        }
    }
}
