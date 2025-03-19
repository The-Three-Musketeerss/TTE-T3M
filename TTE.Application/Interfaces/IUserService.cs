using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Infrastructure.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponseDto<UserResponseDto>> GetUsers();
        Task<GenericResponseDto<string>> UpdateUser(string username, UpdateUserRequestDto request);
        Task<GenericResponseDto<string>> DeleteUsers(List<string> usernames);

    }
}
