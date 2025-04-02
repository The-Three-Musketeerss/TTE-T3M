using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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

        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequestDto
            {
                Email = "valid@example.com",
                Password = "ValidPassword123"
            };

            var loginData = new LoginResponseDto
            {
                Token = "valid-token",
                Username = "ValidUser",
                Email = "valid@example.com"
            };

            var expectedResponse = new GenericResponseDto<LoginResponseDto>(
                true,
                AuthenticationMessages.MESSAGE_LOGIN_SUCCESS,
                loginData
            );

            _mockAuthService.Setup(s => s.LoginUser(It.IsAny<LoginRequestDto>()))
                            .ReturnsAsync(expectedResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<LoginResponseDto>>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal(loginData.Token, ((LoginResponseDto)response.Data).Token);
            Assert.Equal(loginData.Username, ((LoginResponseDto)response.Data).Username);
            Assert.Equal(loginData.Email, ((LoginResponseDto)response.Data).Email);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
        {
            var loginRequest = new LoginRequestDto
            {
                Email = "invalid@example.com",
                Password = "WrongPassword"
            };

            var failedResponse = new GenericResponseDto<LoginResponseDto>(
                false,
                AuthenticationMessages.MESSAGE_LOGIN_FAIL
            );

            _mockAuthService.Setup(s => s.LoginUser(It.IsAny<LoginRequestDto>()))
                            .ReturnsAsync(failedResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<GenericResponseDto<LoginResponseDto>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.Equal(AuthenticationMessages.MESSAGE_LOGIN_FAIL, response.Message);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnOk_WhenAdmin()
        {
            // Arrange
            SetUserRole(AppConstants.ADMIN);

            var request = new EmployeeRequestDto
            {
                Name = "Admin User",
                UserName = "adminuser",
                Email = "admin@example.com",
                Password = "AdminPass123"
            };

            var response = new GenericResponseDto<EmployeeResponseDto>(
                true,
                AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS,
                new EmployeeResponseDto
                {
                    Id = 1,
                    UserName = request.UserName,
                    Email = request.Email,
                    Role = "Admin"
                }
            );

            _mockAuthService.Setup(service => service.RegisterEmployee(request))
                            .ReturnsAsync(response);

            // Act
            var result = await _authController.RegisterEmployee(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var responseData = Assert.IsType<GenericResponseDto<EmployeeResponseDto>>(result.Value);
            Assert.True(responseData.Success);

            Assert.Equal(AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, responseData.Message);
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
