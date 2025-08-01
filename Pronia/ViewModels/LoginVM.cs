using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels;

public class LoginVM
{
    public string UsernameOrEmail { get; set; } = null!;
    
    [Required, DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}