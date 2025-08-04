using Pronia.Models;

namespace Pronia.ViewModels;

public class ShopIndexVM
{
    public IEnumerable<ProductListItemVM> Products { get; set; } = new List<ProductListItemVM>();
    
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    
    public IEnumerable<Color> Colors { get; set; } = new List<Color>();
}