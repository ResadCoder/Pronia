using Pronia.Utilities;

namespace Pronia.Models;

public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public int Price { get; set;}
    
    public int SKU { get; set; }
    
    public int CategoryId { get; set; }
    
    public Category Category { get; set; }
    
    public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    
    public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    
    public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    
}