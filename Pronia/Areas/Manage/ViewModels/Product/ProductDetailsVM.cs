using Pronia.Models;

namespace Pronia.Areas.Manage.ViewModels.Product;

public class ProductDetailsVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Sku { get; set; }
    public string CategoryName { get; set; } = null!;
    
    public IEnumerable<string> Sizes { get; set; } = null!;
    
    public IEnumerable<string> Colors { get; set; } = null!;
    
    
    public string MainImagePath { get; set; } = null!;
    public string HoverImagePath { get; set; } = null!;
    public IEnumerable<string> AdditionalImages { get; set; } = new List<string>();
}