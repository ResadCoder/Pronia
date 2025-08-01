using System.ComponentModel.DataAnnotations;
using Pronia.Attributes;

namespace Pronia.ViewModels;

public class RegisterVM
{
    [MinLength(3)]
    public string Name { get; set; } = null!;

    [MinLength(3)]
    public string Surname { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    [EmailCheck]
    public string Email { get; set;} = null!;
    
    [DataType(DataType.Password)]
    [Required]
    [MinLength(6)]
    public string Password { get; set; } 
    
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}