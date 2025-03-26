using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WhenAdminCreatesProduct()
        {
            // Arrange
            SetUserRole(AppConstants.ADMIN);

            var request = new ProductRequestDto
            {
                Title = "Test Product",
                Price = 100,
                Description = "Test Description",
                Category = "electronics",
                Image = "img.jpg",
                Inventory = new InventoryDto { Total = 50, Available = 25 }
            };

            var response = new GenericResponseDto<ProductCreatedResponseDto>(true, "", new ProductCreatedResponseDto
            {
                Id = 1,
                Message = "Product created successfully."
            });

            _mockProductService.Setup(service => service.CreateProducts(request, AppConstants.ADMIN))
                               .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateProduct(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnForbid_WhenRoleIsInvalid()
        {
            // Arrange
            SetUserRole("Customer");

            var request = new ProductRequestDto();

            // Act
            var result = await _controller.CreateProduct(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenServiceFails()
        {
            // Arrange
            SetUserRole(AppConstants.EMPLOYEE);

            var request = new ProductRequestDto
            {
                Title = "Test Product",
                Price = 100,
                Description = "Test",
                Category = "invalid",
                Image = "img.jpg",
                Inventory = new InventoryDto { Total = 50, Available = 25 }
            };

            var response = new GenericResponseDto<ProductCreatedResponseDto>(false, "Category not found");

            _mockProductService.Setup(service => service.CreateProducts(request, AppConstants.EMPLOYEE))
                               .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateProduct(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductExists()
        {
            // Arrange
            var response = new GenericResponseDto<ProductByIdResponse>(true, "", new ProductByIdResponse
            {
                Id = 9,
                Title = "WD 2TB External Hard Drive",
                Price = 64,
                Description = "USB 3.0 Compatibility",
                Category = "electronics",
                Image = "img.jpg",
                Rating = new RatingDto { Rate = 3.3, Count = 203 },
                Inventory = new InventoryDto { Total = 50, Available = 25 }
            });

            _mockProductService.Setup(service => service.GetProductById(9))
                               .ReturnsAsync(response);

            // Act
            var result = await _controller.GetProductById(9) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var response = new GenericResponseDto<ProductByIdResponse>(false, "Product not found.");

            _mockProductService.Setup(service => service.GetProductById(99))
                               .ReturnsAsync(response);

            // Act
            var result = await _controller.GetProductById(99) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal(response, result.Value);
        }
    }
}
