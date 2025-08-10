using System;
using System.ComponentModel.DataAnnotations;

public class DateGreaterThanNowAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dt)
        {
            return dt > DateTime.Now;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be greater than the current date and time.";
    }
}