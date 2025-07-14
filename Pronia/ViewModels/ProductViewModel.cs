namespace Pronia.ViewModels.Home;

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
    
    public List<ProductImageViewModel> Images { get; set; } = new List<ProductImageViewModel>();
    
    
}