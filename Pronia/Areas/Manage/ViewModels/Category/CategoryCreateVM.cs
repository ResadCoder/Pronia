using System.ComponentModel.DataAnnotations;
namespace Pronia.Areas.Manage.ViewModels;

public class CategoryCreateVM
{
     [Required]
     [MaxLength(25)]
     public string Name { get; set; } = null!;
}