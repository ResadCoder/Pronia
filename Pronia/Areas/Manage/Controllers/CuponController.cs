using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Coupon;
using Pronia.Areas.Manage.ViewModels.Pagination;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]

public class CuponController(AppDbContext context) : Controller
{
    private const int _pageTake = 5;
    public async Task<IActionResult> Index(int page = 1)
    {
        if (page <= 0) return NotFound();
        int countInDb = await context.Cupons.CountAsync();
        
        PaginationVM<Cupon> vm = new PaginationVM<Cupon>
        {
            CurrentPage = page,
            TotalPageSize = (int)Math.Ceiling((decimal)countInDb / _pageTake),
            Items = await context.Cupons
                .Skip((page - 1) * _pageTake)
                .Take(_pageTake)
                .ToListAsync(),
        };
        
        return View(vm);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CouponCreateVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        
        if (await context.Cupons.AnyAsync(c => c.Code == vm.Code))
        {
            ModelState.AddModelError(nameof(vm.Code), "Coupon already exists");
            return View();
        }

        Cupon cupon = new Cupon
        {
            Code = vm.Code,
            DiscountAmount = vm.DiscountAmount,
            ExpiryDate = vm.ExpirationDate,
            IsActive = vm.IsActive
        };
        await context.Cupons.AddAsync(cupon);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
}