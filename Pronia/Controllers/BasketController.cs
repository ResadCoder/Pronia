using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.Context;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;

namespace Pronia.Controllers;

public class BasketController(AppDbContext _context) : Controller
{
    public async Task<IActionResult> Add(int productId)
    {
        if (productId <= 0) return BadRequest();
        if(!await _context.Products.AnyAsync(p => p.Id == productId)) return BadRequest();
        
        string? basketCookie = HttpContext.Request.Cookies["Basket"];
        List<BasketCookieVM>? items = new List<BasketCookieVM>();

        if (basketCookie != null)
        {
            items = JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);
            
            BasketCookieVM existedCookie = items.FirstOrDefault(i => i.ProductId == productId);
            if (existedCookie != null)
                existedCookie.Count++;
            else
            {
                items.Add(new BasketCookieVM
                {
                    ProductId = productId,
                    Count = 1
                });
            }
        }
        else
        {
            items.Add(new BasketCookieVM
            {
                ProductId = productId,
                Count = 1
            });
        }
        
        Response.Cookies.Append("Basket", JsonConvert.SerializeObject(items));
        return RedirectToAction("Index","Home");
    }

    public IActionResult Get()
    {
        string? basketCookie = Request.Cookies["Basket"];
        List<BasketCookieVM>? items = JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);
        return Json(items);
    }

    public async Task<IActionResult> Index()
    {
        string? basketCookie = Request.Cookies["Basket"];
        if (basketCookie is null)
            return View(new List<BasketItemVM>());
    
        ICollection<BasketCookieVM>? items =
            JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);
        
        List<BasketItemVM> vm = new List<BasketItemVM>();
        
        foreach (BasketCookieVM item in items)
        {
            Product? product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.PositionEnum == ImagePositionEnum.main))
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);
        
            if (product != null)
            {
                vm.Add(new BasketItemVM
                {
                    ProductId = product.Id,
                    Count = item.Count,
                    ProductName = product.Name,
                    MainImage = product.ProductImages.FirstOrDefault()!.ImgPath,
                    Price = product.Price,
                });
            }
        }
        return View(vm);
    }
}