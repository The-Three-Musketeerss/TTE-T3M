using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResponseDto<ShopperResponseDto>> RegisterUser(ShopperRequestDto request);
        Task<GenericResponseDto<UserResponseDto>> RegisterEmployee(EmployeeRequestDto request);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
    }
}
