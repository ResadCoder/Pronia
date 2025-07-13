using Pronia.Utilities;
using Pronia.ViewModels.Home;

namespace Pronia.DTO;

public class ReviewViewModel
{
    
    public string Commentary { get; set; } = null!;
    
    public string UserName { get; set; } = null!;
    
    public string? UserImagePath { get; set; }
    
}