using Pronia.DTO;
using Pronia.Models;

namespace Pronia.ViewModels.Home;

public class HomeIndexViewModel
{
    public List<Slide> Slides { get; set; } = new List<Slide>();
    public List<Card> Cards { get; set; } = new List<Card>();
    
    public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
    
    public List<BlogViewModel> Blogs { get; set; } = new List<BlogViewModel>();
    
    public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    
    public List<ProductImageViewModel> ProductImages { get; set; } = new List<ProductImageViewModel>();
    
    public List<Category> Categories { get; set; } = new List<Category>();
    
    public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    
    public List<Color> Colors { get; set; } = new List<Color>();
    
    public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    
    public  List<Size> Sizes { get; set; } = new List<Size>();

}