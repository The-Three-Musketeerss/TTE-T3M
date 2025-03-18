using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TTE.Commons.Constants;

namespace TTE.Commons.Validators
{
    public class EmailValidator : ValidationAttribute
    {
        private const string EmailPattern = AppConstants.EMAIL_PATTERN_VALIDATOR;
        // Testing if my commits are linked to my account
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value.ToString();

            if (!Regex.IsMatch(email, EmailPattern))
            {
                throw new ValidationException(ValidationMessages.MESSAGE_EMAIL_FAIL);
            }

            return ValidationResult.Success;
        }
    }
}
