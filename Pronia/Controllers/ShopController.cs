using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;

namespace Pronia.Controllers;

public class ShopController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await context.Products
            .Include(p => p.ProductImages)
            .Select(p => new ProductListItemVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                MainImagePath = p.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.main)!.ImgPath
            })
            .ToListAsync();

        var vm = new ShopIndexVM
        {
            Products = products,
            Categories = await context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Products = c.Products 
                })
                .ToListAsync(),

            Colors = await context.Colors
                .Select(c => new Color
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductColors = c.ProductColors 
                })
                .ToListAsync()
        };

        return View(vm);
    }
}