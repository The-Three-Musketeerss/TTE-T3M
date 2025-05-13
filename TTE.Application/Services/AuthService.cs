using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Commons.Services;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class AuthService : IAuthService
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
            if (await _userRepository.GetByCondition(u => u.Email == request.Email) is not null)
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS);

            if (await _userRepository.GetByCondition(u => u.UserName == request.UserName) is not null)
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_USERNAME_ALREADY_EXISTS);

            if (await _securityQuestionRepository.GetByCondition(s => s.Id == request.SecurityQuestionId) is null)
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_INVALID_SECURITY_QUESTION_ID);

            var role = await _roleRepository.GetByCondition(r => r.Name == AppConstants.SHOPPER);
            if (role == null)
                return new GenericResponseDto<ShopperResponseDto>(false, ValidationMessages.MESSAGE_ROLE_NOT_FOUND);

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Name = request.Name,
                Password = _securityService.HashPassword(request.Password),
                SecurityQuestionId = request.SecurityQuestionId,
                SecurityAnswer = _securityService.HashPassword(request.SecurityAnswer),
                RoleId = role.Id
            };

            await _userRepository.Add(newUser);

            return new GenericResponseDto<ShopperResponseDto>(true, AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, new ShopperResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.Name,
                UserName = newUser.UserName,
                RoleId = newUser.RoleId
            });
        }

        public async Task<GenericResponseDto<LoginResponseDto>?> LoginUser(LoginRequestDto request)
        {
            var user = await _userRepository.GetByCondition(
                u => u.Email == request.Email,
                AppConstants.ROLE
            );

            if (user == null || !_securityService.VerifyPassword(request.Password, user.Password))
                return null;

            var token = _securityService.GenerateToken(user.UserName, user.Role.Name, user.Id);

            return new GenericResponseDto<LoginResponseDto>(true, AuthenticationMessages.MESSAGE_LOGIN_SUCCESS, new LoginResponseDto
            {
                Token = token,
                Username = user.UserName,
                Email = user.Email
            });
        }

        public async Task<GenericResponseDto<EmployeeResponseDto>> RegisterEmployee(EmployeeRequestDto request)
        {
            if (await _userRepository.GetByCondition(u => u.Email == request.Email) is not null)
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_EMAIL_ALREADY_EXISTS);

            if (await _userRepository.GetByCondition(u => u.UserName == request.UserName) is not null)
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_USERNAME_ALREADY_EXISTS);

            var role = await _roleRepository.GetByCondition(r => r.Name == AppConstants.EMPLOYEE);
            if (role == null)
                return new GenericResponseDto<EmployeeResponseDto>(false, ValidationMessages.MESSAGE_ROLE_NOT_FOUND);

            var newUser = new User
            {
                Name = request.Name,
                UserName = request.UserName,
                Email = request.Email,
                Password = _securityService.HashPassword(request.Password),
                RoleId = role.Id
            };

            await _userRepository.Add(newUser);

            return new GenericResponseDto<EmployeeResponseDto>(true, AuthenticationMessages.MESSAGE_SIGN_UP_SUCCESS, new EmployeeResponseDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                UserName = newUser.UserName,
                Role = role.Name
            });
        }

        public async Task<GenericResponseDto<object>> ForgotPassword(ForgotPasswordRequestDto request)
        {
            var user = await _userRepository.GetByCondition(
                u => u.Email == request.Email,
                "SecurityQuestion"
            );

            if (user == null)
                return new GenericResponseDto<object>(false, ValidationMessages.MESSAGE_EMAIL_NOT_FOUND);

            if (user.SecurityQuestionId != request.SecurityQuestionId)
                return new GenericResponseDto<object>(false, ValidationMessages.MESSAGE_INVALID_SECURITY_QUESTION_ID);

            if (!_securityService.VerifyPassword(request.SecurityAnswer, user.SecurityAnswer ?? string.Empty))
                return new GenericResponseDto<object>(false, ValidationMessages.MESSAGE_INVALID_SECURITY_ANSWER);

            user.Password = _securityService.HashPassword(request.NewPassword);
            await _userRepository.Update(user);

            return new GenericResponseDto<object>(true, AuthenticationMessages.MESSAGE_PASSWORD_RESET_SUCCESS);
        }

        public async Task<GenericResponseDto<List<SecurityQuestionDto>>> GetSecurityQuestions()
        {
            var questions = await _securityQuestionRepository.Get();

            var result = questions
                .Select(q => new SecurityQuestionDto
                {
                    Id = q.Id,
                    Question = q.Question
                })
                .ToList();

            return new GenericResponseDto<List<SecurityQuestionDto>>(true, "Security questions retrieved successfully.", result);
        }
    }
}
