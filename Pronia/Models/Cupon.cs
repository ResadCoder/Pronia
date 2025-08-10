namespace Pronia.Models;

public class Cupon
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal DiscountAmount { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    
    public int UsageCount { get; set; }
    public int MaxUsage { get; set; }

    public ICollection<CuponUsage> Usages { get; set; } = new List<CuponUsage>();

}