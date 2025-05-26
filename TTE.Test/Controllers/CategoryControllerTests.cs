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
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object);
        }

        private void SetUserRole(string role, string userName = "UnitTestUser")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.Name, userName)
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnOk_WhenAdminCreatesSuccessfully()
        {
            SetUserRole(AppConstants.ADMIN);
            var request = new CategoryRequestDto { Name = "Test Category" };
            var expectedResponse = new GenericResponseDto<CategoryResponseDto>(
                true,
                ValidationMessages.CATEGORY_CREATED_SUCCESSFULLY
            );
            var userName = "UnitTestUser";
            _mockCategoryService
                .Setup(s => s.CreateCategory(request, AppConstants.ADMIN, userName))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _controller.CreateCategory(request);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<CategoryResponseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.CATEGORY_CREATED_SUCCESSFULLY, response.Message);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnForbid_WhenRoleIsInvalid()
        {
            SetUserRole("Customer");

            var request = new CategoryRequestDto();

            // Act
            var result = await _controller.CreateCategory(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnOk()
        {
            var expectedResponse = new GenericResponseDto<CategoryResponseDto>(
                true,
                ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY,
                new List<CategoryResponseDto>
                {
                    new CategoryResponseDto { Id = 1, Name = "Category 1" },
                    new CategoryResponseDto { Id = 2, Name = "Category 2" }
                }
            );

            _mockCategoryService
                .Setup(s => s.GetCategories())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<CategoryResponseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY, response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(2, ((IEnumerable<CategoryResponseDto>)response.Data).Count());
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnOk_WhenAdminDeletesSuccessfully()
        {
            SetUserRole(AppConstants.ADMIN);
            int categoryId = 1;

            var expectedResponse = new GenericResponseDto<string>(
                true,
                ValidationMessages.CATEGORY_DELETED_SUCCESSFULLY
            );

            var userName = "UnitTestUser";

            _mockCategoryService
                .Setup(s => s.DeleteCategory(categoryId, AppConstants.ADMIN, userName))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.CATEGORY_DELETED_SUCCESSFULLY, response.Message);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnBadRequest_WhenCategoryNotFound()
        {
            SetUserRole(AppConstants.ADMIN);
            int categoryId = 99;

            var expectedResponse = new GenericResponseDto<string>(
                false,
                ValidationMessages.CATEGORY_NOT_FOUND
            );

            var userName = "UnitTestUser";

            _mockCategoryService
                .Setup(s => s.DeleteCategory(categoryId, AppConstants.ADMIN, userName))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal(ValidationMessages.CATEGORY_NOT_FOUND, response.Message);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            int categoryId = 1;
            var request = new CategoryRequestDto
            {
                Name = "Updated Category"
            };

            var expectedResponse = new GenericResponseDto<string>(
                true,
                ValidationMessages.CATEGORY_UPDATED_SUCCESSFULLY
            );

            _mockCategoryService
                .Setup(s => s.UpdateCategory(categoryId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateCategory(categoryId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(ValidationMessages.CATEGORY_UPDATED_SUCCESSFULLY, response.Message);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
        {
            int categoryId = 999;
            var request = new CategoryRequestDto
            {
                Name = "Nonexistent Category"
            };

            var expectedResponse = new GenericResponseDto<string>(
                false,
                ValidationMessages.CATEGORY_NOT_FOUND
            );

            _mockCategoryService
                .Setup(s => s.UpdateCategory(categoryId, request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateCategory(categoryId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<string>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal(ValidationMessages.CATEGORY_NOT_FOUND, response.Message);
        }
    }
}
