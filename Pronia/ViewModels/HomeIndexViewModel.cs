using Pronia.DTO;
using Pronia.Models;

namespace Pronia.ViewModels.Home;

public class HomeIndexViewModel
{
    public List<Slide> Slides { get; set; } = new List<Slide>();
    public List<Card> Cards { get; set; } = new List<Card>();
    
    public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
    
    public List<BlogViewModel> Blogs { get; set; } = new List<BlogViewModel>();
    
    public List<HomeIndexProductVM> Products { get; set; } = new List<HomeIndexProductVM>();
    public List<Category> Categories { get; set; } = new List<Category>();
    
}