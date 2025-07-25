using System.ComponentModel.DataAnnotations;
using Pronia.Models;
using Pronia.Models.Attributes;
using Pronia.Utilities;

namespace Pronia.Areas.Manage.ViewModels;

public class ProductCreateVM
{
    
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    
    [Required] [Range(0.01, double.MaxValue)]
    public int Price { get; set; }
    
    [Required] [Range(0.01, double.MaxValue)]
    public decimal Discount { get; set; } 
    
  
    [FileTypeAndSize(2, FileSizeEnum.Mb, FileTypeEnum.Image)]
    public IFormFile MainImage { get; set; } = null!;
  
    [FileTypeAndSize(2, FileSizeEnum.Mb, FileTypeEnum.Image)]
    public IFormFile HoverImage { get; set; } = null!;
    
    [FileTypeAndSize(2, FileSizeEnum.Mb, FileTypeEnum.Image)]
    public List<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();
    public int Sku { get; set; }
    public int CategoryId { get; set; }
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<int> ColorIds { get; set; }
    public List<Models.Color> Colors { get; set; } = new(); 
    public List<int> SizeIds { get; set; } 
    public List<Models.Size> Sizes { get; set; } = new();
   
}