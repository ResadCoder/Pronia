using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Size;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers;
[Area("Manage")]
public class SizeController : Controller
{
    private readonly AppDbContext _context;

    public SizeController(AppDbContext context)
    {
        _context = context;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        List<Size> size = await _context.Sizes.ToListAsync();
        return View(size);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SizeCreateVM vm)
    {
        if (!ModelState.IsValid) return View();
        
        vm.Name = (vm.Name ?? "").Trim();
        if (!string.IsNullOrEmpty(vm.Name))
        {
            vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
        }
        
        if (await _context.Sizes.AnyAsync(c => c.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
        {
            ModelState.AddModelError(nameof(vm.Name), "Size already exists");
            return View();
        }

        Size size = new Size()
        {
            Name = vm.Name
        };
        await _context.Sizes.AddAsync(size);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        if(id<=0)return BadRequest();
        Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
        if (size == null) return NotFound();

        SizeUpdateVM vm = new SizeUpdateVM()
        {
            Name = size.Name,
        };
        
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, SizeUpdateVM vm)
    {
        if(id<=0)return NotFound();
        
        if(!ModelState.IsValid) return View(vm);
        
        Size? size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
        
        if (size == null) return NotFound();
        
        vm.Name = (vm.Name ?? "").Trim();
        if (!string.IsNullOrEmpty(vm.Name))
        {
            vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
        }

        if (!string.Equals(vm.Name, size.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await _context.Sizes.AnyAsync(c => c.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(vm.Name), "Size already exists");
                return View();
            }
            size.Name = vm.Name;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Size updated successfully";
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        if (id<=0) return  BadRequest();
        Size? size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
        if (size == null) return NotFound();
        _context.Sizes.Remove(size);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Size deleted successfully";
        return RedirectToAction(nameof(Index));
    }
    
}