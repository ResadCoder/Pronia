using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Manage.ViewModels.Color;

public class ColorUpdateVM
{
    public int Id { get; set; }
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;
}