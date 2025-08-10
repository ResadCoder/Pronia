using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels;
using Pronia.Areas.Manage.ViewModels.Pagination;
using Pronia.Context;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Areas.Manage.Controllers;


    [Area("Manage")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
   
    public class CategoryController(AppDbContext context) : Controller
    {
        private const int _pageTake = 10;
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page <= 0) return NotFound();
            int countinDb = await context.Categories.CountAsync();
            
            PaginationVM<Category> vm = new PaginationVM<Category>
            {
                CurrentPage = page,
                TotalPageSize = (int)Math.Ceiling((decimal)countinDb / _pageTake),
                Items = await context.Categories
                    .Skip((page-1)*_pageTake)
                    .Take(_pageTake)
                    .ToListAsync()
            };
            return View(vm);
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
            
            if (await context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == vm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(vm.Name), "Category already exists");
                return View();
            } 
            Category category = new Category()
            {
                Name = vm.Name
            };
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            TempData["Success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if(id<=0) return BadRequest();
            
            Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
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
            
            Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            
            if(category == null) return NotFound();
            
            vm.Name = (vm.Name ?? "").Trim();
            if (!string.IsNullOrEmpty(vm.Name))
            {
                vm.Name = char.ToUpper(vm.Name[0]) + vm.Name.Substring(1).ToLower();
            }

            if (!string.Equals(vm.Name , category.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await context.Categories.AnyAsync(c => c.Name.ToLower() == vm.Name.ToLower()))
                {
                   ModelState.AddModelError(nameof(vm.Name), "Category already exists");
                   return View();
                }
                category.Name = vm.Name;
                await context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            
            Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if(category == null) return NotFound();
            
            context.Categories.Remove(category);
            TempData["DeleteWarn"] = "Are you sure you want to delete this category? ";
            await context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
    