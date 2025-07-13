using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Manage.ViewModels.Color;

public class ColorCreateVM
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;
}