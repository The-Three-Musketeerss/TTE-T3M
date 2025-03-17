using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TTE.Infrastructure.Validators
{
    public class EmailValidator : ValidationAttribute
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value.ToString();

            if (!Regex.IsMatch(email, EmailPattern))
            {
                return new ValidationResult("The email format is invalid.");
            }

            return ValidationResult.Success;
        }
    }
}
