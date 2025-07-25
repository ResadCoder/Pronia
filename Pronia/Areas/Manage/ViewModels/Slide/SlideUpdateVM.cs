namespace Pronia.Areas.Manage.ViewModels.Slide;

public class SlideUpdateVM
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Subtitle { get; set; } = null!;
        
    public int Order { get; set; } 
    
    public bool ShowSlide { get; set; }
    
    public string? ImagePath { get; set; }
    
    public IFormFile? Photo { get; set; } 
    
}