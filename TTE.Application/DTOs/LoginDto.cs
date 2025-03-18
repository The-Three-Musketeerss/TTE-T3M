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

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
