using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Color;
using Pronia.Context;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]
public class ColorController(AppDbContext context) : Controller
{
    private readonly AppDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        List<Color> colors = await _context.Colors.ToListAsync();
        return View(colors);
    }

    public IActionResult Create()
    {
        return View();
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Create(ColorCreateVM vm)
    {
        if (!ModelState.IsValid) return View();
        
        vm.Name = (vm.Name ?? "").Trim();
        if (!string.IsNullOrEmpty(vm.Name))
        {
            vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
        }
        
        if (await _context.Colors.AnyAsync(c => c.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
        {
            ModelState.AddModelError(nameof(vm.Name), "Color already exists");
            return View();
        }

        Color color = new Color()
        {
            Name = vm.Name
        };
        await _context.Colors.AddAsync(color);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Color created successfully";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        if (id <=0) return  BadRequest();
        
        Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
        
        if (color == null) return NotFound();

        ColorUpdateVM vm = new ColorUpdateVM
        {
            Name = color.Name
        };
        return View(vm);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Update(int id, ColorUpdateVM vm)
    {
        if(id<=0) return  BadRequest();
        if (!ModelState.IsValid) return View();
        Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
        if (color == null) return NotFound();
        
        vm.Name = (vm.Name ?? "").Trim();
        if (!string.IsNullOrEmpty(vm.Name))
        {
            vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
        }
        
        if (!string.Equals(vm.Name , color.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _context.Colors.AnyAsync(c => c.Name.ToLower() == vm.Name.ToLower()))
            {
                ModelState.AddModelError(nameof(vm.Name), "Color already exists");
                return View();
            }
            color.Name = vm.Name;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Color updated successfully";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (id<=0) return  BadRequest();
        Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
        if (color == null) return NotFound();
        _context.Colors.Remove(color);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Category deleted successfully";
        return RedirectToAction(nameof(Index));
    }

}