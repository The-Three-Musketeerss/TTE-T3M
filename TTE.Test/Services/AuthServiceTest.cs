using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;
using Moq;
using Xunit;
using TTE.Application.Services;
using TTE.Commons.Services;
using TTE.Application.DTOs;
using TTE.Commons.Constants;
using TTE.Infrastructure.DTOs;

namespace TTE.Tests.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<IGenericRepository<User>> _mockUserRepository;
        private readonly Mock<IGenericRepository<Role>> _mockRoleRepository;
        private readonly Mock<IGenericRepository<SecurityQuestion>> _mockSecurityQuestionRepository;
        private readonly Mock<ISecurityService> _mockSecurityService;
        private readonly AuthService _authService;

        public AuthServiceTest() {
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
            Assert.Equal(ValidationMessages.MESSAGE_ROL_NOT_FOUND, result.Message);
            Assert.Null(result.Data);
        }
    }
}
