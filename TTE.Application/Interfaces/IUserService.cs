using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponseDto<UserResponseDto>> GetUsers();
    }
}
