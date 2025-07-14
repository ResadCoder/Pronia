namespace Pronia.Models;

public class Size
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}