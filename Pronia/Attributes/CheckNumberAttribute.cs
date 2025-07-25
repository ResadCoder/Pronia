using System.ComponentModel.DataAnnotations;

namespace Pronia.Models.Attributes;

public class CheckNumberAttribute : ValidationAttribute
{
    private readonly int _min;
    private readonly int _max;
    
    public CheckNumberAttribute(int min = 0, int max = int.MaxValue)
    {
        _min = min;
        _max = max;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int converted)
        {
            if(converted >= _min && converted <= _max )
                return ValidationResult.Success;
        } 
        return new ValidationResult("Invalid number or type");
    }
}