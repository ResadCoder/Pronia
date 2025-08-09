using Pronia.Models;

namespace Pronia.ViewModels;

public class QuickViewProductVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string MainImage { get; set; }

    public List<ColorVM> Colors { get; set; } = new();
    public List<SizeVM> Sizes { get; set; } = new();
}