using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class ShopperRequestDto
    {
        [EmailValidator, RequiredFieldValidator]
        public string Email { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string Name { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string UserName { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public string Password { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public int SecurityQuestionId { get; set; }
        [RequiredFieldValidator]
        public string SecurityAnswer { get; set; } = string.Empty;
    }
}
