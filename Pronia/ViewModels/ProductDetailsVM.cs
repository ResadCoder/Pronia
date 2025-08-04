using Pronia.Models;

namespace Pronia.ViewModels;

public class ProductDetailsVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public int Price { get; set;}
    
    public int SKU { get; set; }
    
    public int CategoryId { get; set; }
    
    public decimal Discount { get; set; }
    
    public Category Category { get; set; }
    
    public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    
    public List<ProductColor> Colors { get; set; } = new List<ProductColor>();

    public List<ProductSize> Sizes { get; set; } = new List<ProductSize>();
    
    public ICollection<ProductListItemVM> RelatedProducts { get; set; } = new List<ProductListItemVM>();
}