namespace Pronia.Areas.Manage.ViewModels.Product;

public class ProductIndexVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public  string MainImage { get; set; } = null!;
    
}