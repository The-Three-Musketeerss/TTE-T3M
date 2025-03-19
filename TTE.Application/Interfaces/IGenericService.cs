using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Infrastructure.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IGenericService<T, T2>
    {
        Task<GenericResponseDto<T2>> GetAll();

    }
}
