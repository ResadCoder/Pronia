using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Manage.ViewModels.Coupon;

public class CouponCreateVM
{
    [Required]
    [MaxLength (15)]
    public string Code { get; set; }

    [Required]
    public decimal DiscountAmount { get; set; } 
    
    [Required]
    [DateGreaterThanNow]
    public DateTime ExpirationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
}