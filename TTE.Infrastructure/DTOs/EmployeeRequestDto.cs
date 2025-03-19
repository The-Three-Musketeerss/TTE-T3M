using TTE.Commons.Validators;

namespace TTE.Infrastructure.DTOs
{
    public class EmployeeRequestDto
    {
        [RequiredFieldValidator]
        public string Name { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string UserName { get; set; } = string.Empty;
        [EmailValidator, RequiredFieldValidator]
        public string Email { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string Password { get; set; } = string.Empty;
    }
}
