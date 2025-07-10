namespace Pronia.Models;

public class Review
{
    public int Id { get; set; }
    
    public string UserName { get; set; } = null!;
    public string Commentary { get; set; }
    
    public string? UserImagePath { get; set; } 
}