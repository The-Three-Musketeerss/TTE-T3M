using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Services;
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
                return new GenericResponseDto<string>(false, "User not found");
            }

            user.UserName = request.Name;
            user.Email = request.Email;
            user.Password = _securityService.HashPassword(request.Password);

            await _userRepository.Update(user);

            return new GenericResponseDto<string>(true, $"User {username} has been updated successfully.");
        }
    }
}
