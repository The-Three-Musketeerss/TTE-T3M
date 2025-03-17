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
            if (value == null)
            {
                return new ValidationResult("El correo electrónico es obligatorio.");
            }

            string email = value.ToString();

            if (!Regex.IsMatch(email, EmailPattern))
            {
                return new ValidationResult("El formato del correo electrónico no es válido.");
            }

            return ValidationResult.Success;
        }
    }
}
