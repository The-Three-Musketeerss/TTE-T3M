using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnOk_WhenValidRequest()
        {
            var request = new ShopperRequestDto
            {
                Email = "test@example.com",
                UserName = "TestUser",
                Password = "SecurePass123",
                SecurityQuestionId = 1,
                SecurityAnswer = "TestAnswer"
            };

            var response = new GenericResponseDto<ShopperResponseDto>(true, "User registered successfully", new ShopperResponseDto
            {
                Id = 1,
                Email = request.Email,
                UserName = request.UserName,
                RoleId = 3
            });

            _mockAuthService.Setup(service => service.RegisterUser(request))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.RegisterUser(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {

            var request = new ShopperRequestDto
            {
                Email = "existing@example.com",
                UserName = "ExistingUser",
                Password = "SecurePass123",
                SecurityQuestionId = 1,
                SecurityAnswer = "TestAnswer"
            };

            var response = new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS);

            _mockAuthService.Setup(service => service.RegisterUser(request))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.RegisterUser(request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

    }
}
