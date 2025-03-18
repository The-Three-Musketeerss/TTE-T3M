using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Infrastructure.Validators;

namespace TTE.Application.DTOs
{
    public class EmployeeRequestDto
    {
        [RequiredFieldValidator]
        public string UserName { get; set; } = string.Empty;
        [EmailValidator, RequiredFieldValidator]
        public string Email { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string Password { get; set; } = string.Empty;
    }
}
