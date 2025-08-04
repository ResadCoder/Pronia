using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;

namespace Pronia.Controllers;

public class ProductController(AppDbContext context) : Controller
{
   public IActionResult Index()
   {
      return View();
   }

   public async Task<IActionResult> Details(int id)
   {
      if (id <= 0)
      {
         return BadRequest();
      }
      
      var product = await context.Products
         .Include(p => p.Category)
         .Include(p => p.ProductImages)
         .Include(p => p.Colors)
            .ThenInclude(pc => pc.Color)
         .Include(p => p.Sizes)
            .ThenInclude(ps => ps.Size)
         .FirstOrDefaultAsync(p => p.Id == id);
     
      if (product == null) return NotFound();
      
      var relatedProducts = await context.Products
         .Include(p => p.ProductImages)
         .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
         .Take(4)
         .Select(p => new ProductListItemVM
         {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            MainImagePath = p.ProductImages
               .FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.main)!.ImgPath
         }).ToListAsync();
      var vm = new ProductDetailsVM
      {
         Id = product.Id,
         Name = product.Name,
         Description = product.Description,
         Price = product.Price,
         SKU = product.SKU,
         CategoryId = product.CategoryId,
         Discount = product.Discount,
         Category = product.Category,
         ProductImages = product.ProductImages.ToList(),
         Colors = product.Colors.ToList(),
         Sizes = product.Sizes.ToList(),
         RelatedProducts = relatedProducts
      };
      
      return View(vm);
   }
    
}