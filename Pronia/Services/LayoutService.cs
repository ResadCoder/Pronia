using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.Context;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;

namespace Pronia.Services;

public class LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor)
{
    public async Task<Dictionary<string, string>> GetSettingsAsync()
    {
        return await context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
    }

    public async Task<List<BasketItemVM>> GetBasketItemsAsync()
    {
        string? basketCookie = contextAccessor.HttpContext?.Request.Cookies["Basket"];
        List<BasketItemVM> vm = new List<BasketItemVM>();
        
        if (basketCookie is null)
            return vm;
        
        ICollection<BasketCookieVM>? items =
            JsonConvert.DeserializeObject<List<BasketCookieVM>>(basketCookie);

        if (items == null)
        {
            contextAccessor.HttpContext?.Response.Cookies.Delete("Basket");
            return vm;
        }
        
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
        return vm;
    }
}