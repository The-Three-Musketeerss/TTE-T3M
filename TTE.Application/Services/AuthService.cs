﻿using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Commons.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class AuthService: IAuthService
    {


        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<SecurityQuestion> _securityQuestionRepository;
        private readonly ISecurityService _securityService;


        public AuthService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<SecurityQuestion> securityQuestionRepository,
            ISecurityService securityService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _securityQuestionRepository = securityQuestionRepository;
            _securityService = securityService;
        }

        public async Task<GenericResponseDto<ShopperResponseDto>> RegisterUser(ShopperRequestDto request)
        {
            var requestData = request;

            var user = await _userRepository.GetByCondition(u => u.Email == requestData.Email);
            if (user != null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS);
            }
            var user1 = await _userRepository.GetByCondition(u => u.UserName == requestData.UserName);
            if (user1 != null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_USERNAME_ALREADY_EXISTS);
            }
            var securityQuestion = await _securityQuestionRepository.GetByCondition(s => s.Id == requestData.SecurityQuestionId);
            if (securityQuestion == null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_INVALID_SECURITY_QUESTION_ID);
            }

            var role = await _roleRepository.GetByCondition(r => r.Name == AppConstants.SHOPPER);
            
            if (role == null)
            {
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_ROLE_NOT_FOUND);
            }

            var newUser = new User
            {
                UserName = requestData.UserName,
                Email = requestData.Email,
                Name = requestData.Name,
                Password = _securityService.HashPassword(requestData.Password),
                SecurityQuestionId = requestData.SecurityQuestionId,
                SecurityAnswer = _securityService.HashPassword(requestData.SecurityAnswer),
                RoleId = role.Id,
            };

            await _userRepository.Add(newUser);

            var response = new ShopperResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.Name,
                UserName = newUser.UserName,
                RoleId = newUser.RoleId,
            };

            return new GenericResponseDto<ShopperResponseDto>(true, AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, response);
        }

        public async Task<GenericResponseDto<LoginResponseDto>?> LoginUser(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByCondition(u => u.Email == loginRequest.Email, AppConstants.ROLE);

            if (user == null || !_securityService.VerifyPassword(loginRequest.Password, user.Password))
                return null;

            var token = _securityService.GenerateToken(user.UserName, user.Role.Name, user.Id);

            var response = new LoginResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email
            };

            return new GenericResponseDto<LoginResponseDto>(true, AuthenticationMessages.MESSAGE_LOGIN_SUCCESS, response);
        }

        public async Task<GenericResponseDto<EmployeeResponseDto>> RegisterEmployee(EmployeeRequestDto request)
        {
            var requestData = request;
            var role = await _roleRepository.GetByCondition(r => r.Name == AppConstants.EMPLOYEE);

            var user = await _userRepository.GetByCondition(u => u.Email == requestData.Email);
            if (user != null)
            {
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS);
            }

            user = await _userRepository.GetByCondition(u => u.UserName == requestData.UserName);
            if (user != null)
            {
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_USERNAME_ALREADY_EXISTS);
            }

            if (role == null)
            {
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_ROLE_NOT_FOUND);
            }
            user = new User
            {
                Name = requestData.Name,
                UserName = requestData.UserName,
                Email = requestData.Email,
                Password = _securityService.HashPassword(requestData.Password),
                RoleId = role.Id,
            };
            await _userRepository.Add(user);
            var response = new EmployeeResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = role.Name,
            };
            return new GenericResponseDto<EmployeeResponseDto>(true, AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, response);
        }

    }
}
