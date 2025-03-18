using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class LoginRequestDto
    {
        [EmailValidator, RequiredFieldValidator]
        public string Email { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string Password { get; set; } = string.Empty;
    }
}
