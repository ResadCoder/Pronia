using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels;

public class RegisterVM
{
    [MinLength(3)]
    public string Name { get; set; } = null!;

    [MinLength(3)]
    public string Surname { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}