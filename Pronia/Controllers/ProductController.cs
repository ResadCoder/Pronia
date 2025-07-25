using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Controllers;

public class ProductController : Controller
{
   private  readonly AppDbContext _context; 
   public ProductController(AppDbContext Context)
   {
      _context = Context;
   }
   
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
      Product? product = await _context.Products
         .Include(p => p.ProductImages)
         .Include(p => p.Category)
         .Include(p => p.Colors)
            .ThenInclude(pc => pc.Color)
         .Include(p => p.Sizes)
            .ThenInclude(ps => ps.Size)
         .FirstOrDefaultAsync(p => p.Id == id);
      if (product == null)
      {
         return NotFound();
      }
      
      return View(product);
   }
    
}