using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResponseDto<ShopperResponseDto>> RegisterUser(ShopperRequestDto request);
        Task<GenericResponseDto<LoginResponseDto>?> LoginUser(LoginRequestDto loginRequest);
        Task<GenericResponseDto<EmployeeResponseDto>> RegisterEmployee(EmployeeRequestDto request);

    }
}
