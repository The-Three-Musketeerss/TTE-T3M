using System.ComponentModel.DataAnnotations;

namespace TTE.Infrastructure.Validators
{
    public class RequiredFieldValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                string fieldName = validationContext.DisplayName;
                return new ValidationResult($"{fieldName} is required");
            }

            return ValidationResult.Success;
        }
    }
}
