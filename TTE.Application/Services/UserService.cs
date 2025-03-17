using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Application.Utils;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class UserService : IUserService
    {

        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<SecurityQuestion> _securityQuestionRepository;


        public UserService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<SecurityQuestion> securityQuestionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _securityQuestionRepository = securityQuestionRepository;
        }

        public async Task<GenericResponseDto<ShopperResponseDto>> RegisterUser(GenericRequestDto<ShopperRequestDto> request)
        {
            var requestData = request.Data;

            var securityQuestion = await _securityQuestionRepository.GetById(requestData.SecurityQuestionId);
            if (securityQuestion == null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, "Invalid security question ID", null);
            }

            var role = await _roleRepository.GetByCondition(r => r.Name == "shopper");
            if (role == null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, "Role not found", null);
            }

            var user = new User
            {
                UserName = requestData.UserName,
                Email = requestData.Email,
                Password = PasswordHasher.HashPassword(requestData.Password),
                SecurityQuestionId = requestData.SecurityQuestionId,
                SecurityAnswer = PasswordHasher.HashPassword(requestData.SecurityAnswer),
                RoleId = role.Id,
            };

            await _userRepository.Add(user);

            var response = new ShopperResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                RoleId = user.RoleId,
            };

            return new GenericResponseDto<ShopperResponseDto>(true, "User registered successfully", response);
        }
    }
}
