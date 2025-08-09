namespace Pronia.ViewModels;

public class BasketItemVM
{
     public int ProductId { get; set; }
     public int Count { get; set; }
     public string ProductName { get; set; } = null!;
     public decimal Price { get; set; }
     public string MainImage { get; set; } = null!;
     public decimal Subtotal => Price * Count;
}