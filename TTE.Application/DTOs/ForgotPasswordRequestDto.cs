namespace TTE.Application.DTOs
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public int SecurityQuestionId { get; set; }
        public string SecurityAnswer { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
