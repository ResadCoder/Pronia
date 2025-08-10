namespace Pronia.Models;

public class CuponUsage
{
    public int Id { get; set; }

    public int CouponId { get; set; }
    public Cupon Coupon { get; set; } = null!;

    public int UserId { get; set; } 
    public User User { get; set; } = null!;

    public DateTime UsedDate { get; set; } 
}