using Pronia.Utilities;

namespace Pronia.Models;

public class ProductImage
{
    public int Id { get; set; }
    
    public string ImgPath { get; set; }
    
    public ImagePositionEnum PositionEnum { get; set;}
    
    public int ProductId { get; set; }  
    
    public Product Product { get; set; }
    
}