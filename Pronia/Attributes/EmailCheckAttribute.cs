using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pronia.Attributes;

public class EmailCheckAttribute : ValidationAttribute
{
    private const string Pattern =
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        
        string email = value.ToString()!;

        if (!Regex.IsMatch(email, Pattern))
            return new ValidationResult("Enter right email address.");

        return ValidationResult.Success;
    }
}