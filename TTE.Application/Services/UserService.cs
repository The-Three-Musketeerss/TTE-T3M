using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;

        public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        public async Task<GenericResponseDto<UserResponseDto>> GetUsers()
        {
            var users = await _userRepository.Get();
            var userDtos = users.Select(u => new UserResponseDto
            {
                UserName = u.UserName,
                Email = u.Email,
                Password = u.Password,
                Role = _roleRepository.GetByCondition(r => r.Id == u.RoleId).Result.Name
            }).ToList();

            return new GenericResponseDto<UserResponseDto>(true, "Users retrieved successfully", userDtos);
        }

    }
}
