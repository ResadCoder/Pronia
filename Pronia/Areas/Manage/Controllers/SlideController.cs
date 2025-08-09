using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Slide;
using Pronia.Context;
using Pronia.Extensions;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels.Home;

namespace Pronia.Areas.Manage.Controllers;


    [Area("Manage")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
public class SlideController(AppDbContext context, IWebHostEnvironment environment) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<Slide> slides = await context.Slides.ToListAsync();
        return View(slides);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SlideCreateVM vm)
    {
        if (!ModelState.IsValid) return View();
        if (!vm.Photo.ValidateFileType(FileTypeEnum.Image))
        {
            ModelState.AddModelError(nameof(vm.Photo), "Please provide a valid file format(img)");
            return View();
        }

        const long size = 2; 
        const FileSizeEnum sizeType = FileSizeEnum.Mb;
        if (!vm.Photo.ValidateFileSize(size,sizeType))
        {
            ModelState.AddModelError(nameof(vm.Photo), $"Please provide a valid file length ,size must be less than {size} {sizeType.ToString().ToUpper()}");
            return View();
        }
        
        if (await context.Slides.AnyAsync(s => s.Order == vm.Order))
        {
            ModelState.AddModelError(nameof(vm.Order), "Order already exists");
            return View();
        }
        
        

        Slide slide = new Slide()
        {
            ShowSlide = vm.ShowSlide,
            Order = vm.Order,
            Title = vm.Title,
            Description = vm.Description,
            ImagePath = await vm.Photo.CreateFileAsync(environment.WebRootPath, "assets","images","slider"),
            Subtitle = vm.Subtitle
        };
        
        await context.Slides.AddAsync(slide);
        
        await context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
       
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest();
        
        Slide? slide = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        
        if (slide == null) return NotFound();
        
        slide.ImagePath.DeleteFile(environment.WebRootPath, "assets", "images", "slider");
        
        context.Slides.Remove(slide);
        
        await context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0) return BadRequest();
        Slide? slide = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide == null) return NotFound();

        SlideUpdateVM vm = new SlideUpdateVM()
        {
            Order = slide.Order,
            Title = slide.Title,
            Description = slide.Description,
            ImagePath = slide.ImagePath,
            Subtitle = slide.Subtitle,
            ShowSlide = slide.ShowSlide
        };
        
        return View (vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, SlideUpdateVM vm)
    {
        if (id <= 0) return BadRequest();
        
        
        Slide? slide = await context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        if (slide == null) return NotFound();
        vm.ImagePath = slide.ImagePath;
        if (!ModelState.IsValid) return View(vm);
        
     
        
        if (vm.Photo != null)
        {
            if (!vm.Photo.ValidateFileType(FileTypeEnum.Image))
            {
                ModelState.AddModelError(nameof(vm.Photo), "Please provide a valid file format(img)");
                return View(vm);
            }

            const long size = 2;
            const FileSizeEnum sizeType = FileSizeEnum.Mb;
            if (vm.Photo.ValidateFileSize(size, sizeType))
            {
                ModelState.AddModelError(nameof(vm.Photo),
                    $"Please provide a valid file length, size must be less than {size} {sizeType.ToString().ToUpper()}");
                return View(vm);
            }

        }
        
        if (await context.Slides.AnyAsync(s => s.Order == vm.Order && s.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Order), "Order already exists");
            return View(vm);
        }
        
        
        if (vm.Photo != null)
        {
            slide.ImagePath.DeleteFile(environment.WebRootPath, "assets", "images", "slider");
            slide.ImagePath = await vm.Photo.CreateFileAsync(environment.WebRootPath, "assets", "images", "slider");
        }

        slide.Subtitle = vm.Subtitle;
        slide.ShowSlide = vm.ShowSlide;
        slide.Order = vm.Order;
        slide.Title = vm.Title;
        slide.Description = vm.Description;
        

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
}