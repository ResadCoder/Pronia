using Pronia.Models;

namespace Pronia.Areas.Manage.ViewModels.Product;

public class ProductUpdateVM
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int Price { get; set; }
    
    public decimal Discount { get; set; }
    
    public int Sku { get; set; }
    
    public int CategoryId { get; set; }
    
    public ICollection<int> ColorIds { get; set; } = new List<int>();
    
    public ICollection<int> SizeIds { get; set; } = new List<int>();
    
    public IFormFile? MainImage { get; set; }
    
    public IFormFile? HoverImage { get; set; }
    
    public ICollection<IFormFile>? AdditionalImages { get; set; }
    
    public ICollection<Category>? Categories { get; set; }
    
    public ICollection<Models.Color>? Colors { get; set; } 
    
    public ICollection<Models.Size>? Sizes { get; set; } 
    
    public ICollection<ProductImage>? Images { get; set; }
    
    public ICollection<int> AdditionalImagesIds { get; set; } = new List<int>();
    
}