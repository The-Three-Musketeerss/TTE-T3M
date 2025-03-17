using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(IGenericRepository<User> userRepository, IPasswordHasherService passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByCondition(u => u.Email == loginRequest.Email, "Role");

            if (user == null || !_passwordHasher.VerifyPassword(loginRequest.Password, user.Password))
                return null;

            var token = _jwtService.GenerateToken(user.UserName, user.Role.Name);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email
            };
        }



    }
}
