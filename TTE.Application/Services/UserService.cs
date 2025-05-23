﻿using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Commons.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ISecurityService _securityService;

        public UserService(IGenericRepository<User> userRepository, ISecurityService securityService, IMapper mapper)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _mapper = mapper;
        }
        public async Task<GenericResponseDto<UserResponseDto>> GetUsers()
        {
            var includes = new string[] { "Role" };
            var users = await _userRepository.GetEntityWithIncludes(includes);
            var userDtos = users.Select(u => _mapper.Map<UserResponseDto>(u)).ToList();

            return new GenericResponseDto<UserResponseDto>(true, ValidationMessages.MESSAGE_USERS_RETRIEVED_SUCCESSFULLY, userDtos);
        }

        public async Task<GenericResponseDto<string>> UpdateUser(string username, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByCondition(u => u.UserName == username);
            if (user == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);
            }

            user.Name = request.Name;
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
