using Pronia.Utilities;

namespace Pronia.ViewModels.Home;

public class BlogViewModel
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public string Comment { get; set; }
    
    public string ImgPath { get; set; }
    
    public DateTime Date { get; set; }
    
    public string CreatedBy { get; set; } = null!;
    
}