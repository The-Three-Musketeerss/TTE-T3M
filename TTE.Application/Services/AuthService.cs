using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly ISecurityService _securityService;

        public AuthService(IGenericRepository<User> userRepository, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByCondition(u => u.Email == loginRequest.Email, "Role");

            if (user == null || !_securityService.VerifyPassword(loginRequest.Password, user.Password))
                return null;

            var token = _securityService.GenerateToken(user.UserName, user.Role.Name, user.Id);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email
            };
        }



    }
}
