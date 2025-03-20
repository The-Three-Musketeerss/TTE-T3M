using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Commons.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class UserServiceTests
    {

        private readonly Mock<IGenericRepository<User>> _mockUserRepository;
        private readonly Mock<ISecurityService> _mockSecurityService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IGenericRepository<User>>();
            _mockSecurityService = new Mock<ISecurityService>();
            _mockMapper = new Mock<IMapper>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockSecurityService.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnSuccess_WhenUserExists()
        {

            var username = "testuser";
            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Password = "NewPassword123"
            };

            var user = new User { UserName = username, Email = "old@example.com", Password = "OldPassword" };

            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                               .ReturnsAsync(user);

            _mockSecurityService.Setup(s => s.HashPassword(It.IsAny<string>()))
                                .Returns("hashedPassword");

            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUser(username, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Format(ValidationMessages.MESSAGE_USER_UPDATED_SUCCESSFULLY, username), result.Message);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            var request = new UpdateUserRequestDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Password = "NewPassword123"
            };

            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUser(username, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_USER_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task DeleteUsers_ShouldReturnSuccess_WhenUsersExist()
        {
            // Arrange
            var usernames = new List<string> { "user1", "user2" };
            var usersToDelete = new List<User>
            {
                new User { UserName = "user1", Email = "user1@example.com" },
                new User { UserName = "user2", Email = "user2@example.com" }
            };

            _mockUserRepository.Setup(repo => repo.GetAllByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                               .ReturnsAsync(usersToDelete);

            _mockUserRepository.Setup(repo => repo.Delete(It.IsAny<int>()))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUsers(usernames);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.USER_DELETED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task DeleteUsers_ShouldReturnFailure_WhenUsersNotFound()
        {
            // Arrange
            var usernames = new List<string> { "XAXA" };

            _mockUserRepository.Setup(repo => repo.GetAllByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
                               .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.DeleteUsers(usernames);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_USER_NOT_FOUND, result.Message);
        }
    }
}
