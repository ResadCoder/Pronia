namespace Pronia.Areas.Manage.ViewModels.Slide;

public class SlideCreateVM
{
   
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Subtitle { get; set; } = null!;
    
    public int Order { get; set; } 
    
    public bool ShowSlide { get; set; } = false;
    
    public IFormFile Photo { get; set; } = null!;
}