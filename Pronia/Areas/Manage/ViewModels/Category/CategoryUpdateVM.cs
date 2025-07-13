using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Manage.ViewModels;

public class CategoryUpdateVM
{
    public int Id { get; set; }
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = null!;
}