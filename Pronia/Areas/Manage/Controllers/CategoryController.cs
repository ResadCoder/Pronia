using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels;
using Pronia.Context;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Areas.Manage.Controllers;


    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            
            vm.Name = (vm.Name ?? "").Trim();
            if (!string.IsNullOrEmpty(vm.Name))
            {
                vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
            }
            
            if (await _context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(vm.Name), "Category already exists");
                return View();
            } 
            Category category = new Category()
            {
                Name = vm.Name
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if(id<=0) return BadRequest();
            
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
            if(category == null) return NotFound();
            
            CategoryUpdateVM vm = new CategoryUpdateVM()
            {
                Id = category.Id,
                Name = category.Name
            };
            
            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryUpdateVM vm)
        {
            if (!ModelState.IsValid) return View();
            
            if(id<=0) return BadRequest();
            
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
            if(category == null) return NotFound();
            
            vm.Name = (vm.Name ?? "").Trim();
            if (!string.IsNullOrEmpty(vm.Name))
            {
                vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
            }

            if (!string.Equals(vm.Name , category.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == vm.Name.ToLower()))
                {
                   ModelState.AddModelError(nameof(vm.Name), "Category already exists");
                   return View();
                }
                category.Name = vm.Name;
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if(category == null) return NotFound();
            
            _context.Categories.Remove(category);
            TempData["DeleteWarn"] = "Are you sure you want to delete this category? ";
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
    