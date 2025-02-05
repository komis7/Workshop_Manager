using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace WorkShopManager.Validators
{
    public class EmailValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Adres e-mail jest wymagany.");
            }

            // Regularne wyrażenie dla poprawnego formatu e-maila
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(value.ToString()))
            {
                return new ValidationResult("Podaj poprawny adres e-mail (musi zawierać @ i domenę).");
            }

            return ValidationResult.Success;
        }

    }
}
