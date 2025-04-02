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
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _userController = new UserController(_mockUserService.Object);
        }

        private void SetUserRole(string role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk_AdminRole()
        {
            // Arrange
            SetUserRole(AppConstants.ADMIN);
            var response = new GenericResponseDto<UserResponseDto>(
                true,
                ValidationMessages.MESSAGE_USERS_RETRIEVED_SUCCESSFULLY,
                new List<UserResponseDto>()
            );

            _mockUserService
                .Setup(service => service.GetUsers())
                .ReturnsAsync(response);

            // Act
            var result = await _userController.GetAllUsers() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnForbid()
        {
            // Arrange
            SetUserRole(AppConstants.USER);

            // Act
            var result = await _userController.GetAllUsers() as ForbidResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserExists()
        {

            var username = "testuser";
            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Password = "NewPassword123"
            };

            var response = new GenericResponseDto<string>(true, string.Format(ValidationMessages.MESSAGE_USER_UPDATED_SUCCESSFULLY, username));

            _mockUserService.Setup(service => service.UpdateUser(username, request))
                            .ReturnsAsync(response);

            // Act
            var result = await _userController.UpdateUser(username, request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserNotFound()
        {

            var username = "nonexistentuser";
            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Password = "NewPassword123"
            };

            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);

            _mockUserService.Setup(service => service.UpdateUser(username, request))
                            .ReturnsAsync(response);

            // Act
            var result = await _userController.UpdateUser(username, request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task DeleteUsers_ShouldReturnOk_WhenUsersExist()
        {

            var usernames = new List<string> { "user1", "user2" };
            var response = new GenericResponseDto<string>(true, ValidationMessages.USER_DELETED_SUCCESSFULLY);

            _mockUserService.Setup(service => service.DeleteUsers(usernames))
                            .ReturnsAsync(response);

            // Act
            var result = await _userController.DeleteUsers(usernames) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task DeleteUsers_ShouldReturnBadRequest_WhenUsersNotFound()
        {

            var usernames = new List<string> { "NAN" };
            var response = new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);

            _mockUserService.Setup(service => service.DeleteUsers(usernames))
                            .ReturnsAsync(response);

            // Act
            var result = await _userController.DeleteUsers(usernames) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }
    }
}
