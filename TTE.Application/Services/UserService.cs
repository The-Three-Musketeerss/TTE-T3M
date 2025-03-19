using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Commons.Services;
using TTE.Infrastructure.DTOs;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly ISecurityService _securityService;

        public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository; 
            _securityService = securityService;
        }
        public async Task<GenericResponseDto<UserResponseDto>> GetUsers()
        {
            var users = await _userRepository.Get();
            var userDtos = users.Select(u => new UserResponseDto
            {
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name,
                Password = u.Password,
                Role = _roleRepository.GetByCondition(r => r.Id == u.RoleId).Result.Name
            }).ToList();

            return new GenericResponseDto<UserResponseDto>(true, "Users retrieved successfully", userDtos);
        }

        public async Task<GenericResponseDto<string>> UpdateUser(string username, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByCondition(u => u.UserName == username);
            if (user == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);
            }

            user.UserName = request.Name;
            user.Email = request.Email;
            user.Password = _securityService.HashPassword(request.Password);

            await _userRepository.Update(user);

            return new GenericResponseDto<string>(true, string.Format(ValidationMessages.MESSAGE_USER_UPDATED_SUCCESSFULLY, username));
        }

        public async Task<GenericResponseDto<string>> DeleteUsers(List<string> usernames)
        {
            var usersToDelete = await _userRepository.GetAllByCondition(u => usernames.Contains(u.UserName));

            if (usersToDelete == null || !usersToDelete.Any())
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);
            }

            foreach (var user in usersToDelete)
            {
                await _userRepository.Delete(user.Id);
            }

            return new GenericResponseDto<string>(true, ValidationMessages.USER_DELETED_SUCCESSFULLY);
        }
    }
}
