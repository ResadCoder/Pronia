using Microsoft.AspNetCore.Identity;

namespace Pronia.Models;

public class User : IdentityUser<int>
{
    public string Name { get; set; } = null!;
    
    public string Surname { get; set; } = null!;
}