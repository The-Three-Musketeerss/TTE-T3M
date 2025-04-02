using System.Linq.Expressions;
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
    public class AuthServiceTests
    {
        private readonly Mock<IGenericRepository<User>> _mockUserRepository;
        private readonly Mock<IGenericRepository<Role>> _mockRoleRepository;
        private readonly Mock<IGenericRepository<SecurityQuestion>> _mockSecurityQuestionRepository;
        private readonly Mock<ISecurityService> _mockSecurityService;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IGenericRepository<User>>();
            _mockRoleRepository = new Mock<IGenericRepository<Role>>();
            _mockSecurityQuestionRepository = new Mock<IGenericRepository<SecurityQuestion>>();
            _mockSecurityService = new Mock<ISecurityService>();

            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockRoleRepository.Object,
                _mockSecurityQuestionRepository.Object,
                _mockSecurityService.Object
            );
        }

        [Fact]
        public async Task LoginUser_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var request = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "securepass"
            };

            var user = new User
            {
                Id = 1,
                Email = request.Email,
                UserName = "TestUser",
                Password = "hashedPassword",
                Role = new Role { Name = AppConstants.SHOPPER }
            };

            _mockUserRepository.Setup(repo => repo.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), AppConstants.ROLE))
                .ReturnsAsync(user);

            _mockSecurityService.Setup(s => s.VerifyPassword(request.Password, user.Password))
                                .Returns(true);

            _mockSecurityService.Setup(s => s.GenerateToken(user.UserName, user.Role.Name, user.Id))
                                .Returns("mock-token");

            // Act
            var result = await _authService.LoginUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(AuthenticationMessages.MESSAGE_LOGIN_SUCCESS, result.Message);

            var loginData = Assert.IsType<LoginResponseDto>(result.Data);
            Assert.Equal("mock-token", loginData.Token);
            Assert.Equal(user.UserName, loginData.Username);
            Assert.Equal(user.Email, loginData.Email);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenUserNotFound()
        {
            var request = new LoginRequestDto
            {
                Email = "notfound@example.com",
                Password = "irrelevant"
            };

            _mockUserRepository.Setup(repo => repo.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), AppConstants.ROLE))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginUser(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenPasswordIsIncorrect()
        {

            var request = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = 1,
                Email = request.Email,
                UserName = "TestUser",
                Password = "hashedPassword",
                Role = new Role { Name = AppConstants.SHOPPER }
            };

            _mockUserRepository.Setup(repo => repo.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>(), AppConstants.ROLE))
                .ReturnsAsync(user);

            _mockSecurityService.Setup(s => s.VerifyPassword(request.Password, user.Password))
                                .Returns(false);

            // Act
            var result = await _authService.LoginUser(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterShooper_shouldReturnSuccess_whenValidRequest()
        {
            var request = new ShopperRequestDto
            {
                Email = "test@gmail.com",
                UserName = "user",
                Password = "password",
                SecurityQuestionId = 1,
                SecurityAnswer = "answer"
            };

            var role = new Role { Id = 3, Name = AppConstants.SHOPPER };
            var securityQuestion = new SecurityQuestion { Id = 1, Question = AppConstants.SECURITY_QUESTION_1 };

            _mockRoleRepository.Setup(repo => repo.GetByCondition(r => r.Name == AppConstants.SHOPPER))
                               .ReturnsAsync(role);

            _mockSecurityQuestionRepository.Setup(repo => repo.GetByCondition(s => s.Id == request.SecurityQuestionId))
                                           .ReturnsAsync(securityQuestion);

            _mockSecurityService.Setup(s => s.HashPassword(It.IsAny<string>()))
                                .Returns("hashedPassword");

            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                               .ReturnsAsync(1);

            //act
            var result = await _authService.RegisterUser(request);

            //assert
            Assert.NotNull(result.Data);
            Assert.IsType<ShopperResponseDto>(result.Data);
            var userData = result.Data as ShopperResponseDto;
            Assert.NotNull(userData);
            Assert.True(result.Success);
            Assert.Equal(AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, result.Message);

            Assert.Equal(request.Email, userData.Email);
            Assert.Equal(request.UserName, userData.UserName);
        }

        [Fact]
        public async Task RegisterUser_ShouldFail_WhenSecurityQuestionNotFound()
        {
            var request = new ShopperRequestDto
            {
                Email = "test@example.com",
                UserName = "TestUser",
                Password = "SecurePass123",
                SecurityQuestionId = 99,
                SecurityAnswer = "TestAnswer"
            };

            _mockSecurityQuestionRepository.Setup(repo => repo.GetByCondition(s => s.Id == request.SecurityQuestionId))
                                           .ReturnsAsync((SecurityQuestion)null);

            // Act
            var result = await _authService.RegisterUser(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_INVALID_SECURITY_QUESTION_ID, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldReturnSuccess()
        {
            // Arrange
            var request = new EmployeeRequestDto
            {
                Email = "employee@example.com",
                UserName = "EmployeeUser",
                Password = "SecurePass123",
                Name = "Employee Name"
            };

            var role = new Role { Id = 2, Name = AppConstants.EMPLOYEE };
            _mockRoleRepository.Setup(r => r.GetByCondition(r => r.Name == AppConstants.EMPLOYEE))
                               .ReturnsAsync(role);

            _mockSecurityService
                .Setup(s => s.HashPassword(request.Password))
                .Returns("hashedPassword");

            _mockUserRepository
                .Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterEmployee(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, result.Message);
            var userData = Assert.IsType<EmployeeResponseDto>(result.Data);
            Assert.Equal(request.Email, userData.Email);
            Assert.Equal(request.UserName, userData.UserName);
        }

        [Fact]
        public async Task RegisterEmployee_ShouldFail_EmailinUse()
        {
            // Arrange
            var request = new EmployeeRequestDto
            {
                Email = "employee@example.com",
                UserName = "EmployeeUser",
                Password = "SecurePass123",
                Name = "Employee Name"
            };

            // First, ensure role is found so we don't fail early
            var role = new Role { Id = 2, Name = AppConstants.EMPLOYEE };
            _mockRoleRepository.Setup(r => r.GetByCondition(r => r.Name == AppConstants.EMPLOYEE))
                               .ReturnsAsync(role);

            var existingUser = new User
            {
                Id = 1,
                Email = request.Email,
                UserName = "ExistingUser",
                Password = "hashedPassword",
                Role = role
            };

            // Force the repository to return an existing user matching this email
            _mockUserRepository.Setup(repo => repo.GetByCondition(
                It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(existingUser);


            // Act
            var result = await _authService.RegisterEmployee(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task RegisterUser_ShouldFail_WhenRoleNotFound()
        {
            var request = new ShopperRequestDto
            {
                Email = "test@example.com",
                UserName = "TestUser",
                Password = "SecurePass123",
                SecurityQuestionId = 1,
                SecurityAnswer = "TestAnswer"
            };

            var securityQuestion = new SecurityQuestion { Id = 1, Question = AppConstants.SECURITY_QUESTION_1 };

            _mockSecurityQuestionRepository.Setup(repo => repo.GetByCondition(s => s.Id == request.SecurityQuestionId))
                                           .ReturnsAsync(securityQuestion);

            _mockRoleRepository.Setup(repo => repo.GetByCondition(r => r.Name == AppConstants.SHOPPER))
                               .ReturnsAsync((Role)null);

            // Act
            var result = await _authService.RegisterUser(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_ROLE_NOT_FOUND, result.Message);
            Assert.Null(result.Data);
        }
    }
}
