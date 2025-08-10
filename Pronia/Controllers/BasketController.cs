using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.Context;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
  public class BasketController(AppDbContext context, UserManager<User> userManager) : Controller
  {
      public async Task<IActionResult> Add(int productId)
    {
        if (productId <= 0) return BadRequest();
        if (!await context.Products.AnyAsync(p => p.Id == productId)) return BadRequest();

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

        Response.Cookies.Append("Basket", JsonConvert.SerializeObject(items), new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Secure = true
        });
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Get()
    {
        string? basketCookie = Request.Cookies["Basket"];
        List<BasketCookieVM>? items = JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);
        return Json(items);
    }

    public async Task<IActionResult> Index(string couponCode = "" , decimal couponAmount = 0)
    {
        ViewBag.CouponCode = couponCode;
        string? basketCookie = Request.Cookies["Basket"];
        if (basketCookie is null)
            return View(new List<BasketItemVM>());

        ICollection<BasketCookieVM>? items =
            JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);

        if (items == null)
        {
            Response.Cookies.Delete("Basket");
            return View(new List<BasketItemVM>());
        }

        List<BasketItemVM> vm = new List<BasketItemVM>();

        foreach (BasketCookieVM item in items)
        {
            Product? product = await context.Products
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

        decimal total = vm.Sum(x => x.Subtotal)-couponAmount;
        
        ViewBag.Total = total;
        
        return View(vm);
    }

    public IActionResult Remove(int productId)
    {
        string? basketCookie = Request.Cookies["Basket"];

        if (string.IsNullOrEmpty(basketCookie))
            return RedirectToAction("Index");

        List<BasketCookieVM> items = JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie) ?? new List<BasketCookieVM>();

        BasketCookieVM? itemToRemove = items.FirstOrDefault(i => i.ProductId == productId);

        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
        }

        if (items.Count > 0)
        {
            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(items), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true
            });
        }
        else
        {
            Response.Cookies.Delete("Basket");
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ApplyCoupon(string code)
    {
        Cupon? coupon = await context.Cupons
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive && (c.ExpiryDate == null || c.ExpiryDate > DateTime.Now));

        if (coupon == null)
        {
            TempData["ErrorMessage"] = "Invalid Coupon";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["SuccessMessage"] = $"Coupon is valid! Discount: {coupon.DiscountAmount}";
        }
        
        return RedirectToAction("Index",new {couponCode=code, couponAmount=coupon.DiscountAmount});
    }
}
}