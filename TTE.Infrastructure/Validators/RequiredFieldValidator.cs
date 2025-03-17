using System.ComponentModel.DataAnnotations;
using TTE.Commons.Constants;

namespace TTE.Infrastructure.Validators
{
    public class RequiredFieldValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                string fieldName = validationContext.DisplayName;
               throw new ValidationException(ValidationMessages.MESSAGE_REQUIRED_FIELD + $"{fieldName}");
            }

            return ValidationResult.Success;
        }
    }
}
