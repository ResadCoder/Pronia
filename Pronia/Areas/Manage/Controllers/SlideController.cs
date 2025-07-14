using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Manage.ViewModels.Slide;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]
public class SlideController : Controller
{
    private AppDbContext _context;
    private IWebHostEnvironment _environment;
    public SlideController(AppDbContext context,IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
    public async Task<IActionResult> Index()
    {
        List<Slide> slides = await _context.Slides.ToListAsync();
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
        if (!vm.Photo.ContentType.Contains("image/"))
        {
            ModelState.AddModelError(nameof(vm.Photo), "Please provide a valid file format(img)");
            return View();
        }

        if (vm.Photo.Length > 1024 * 1024 * 2)
        {
            ModelState.AddModelError(nameof(vm.Photo), "Please provide a valid file length ,size must be less than 2MB");
            return View();
        }
        if (await _context.Slides.AnyAsync(s => s.Order == vm.Order))
        {
            ModelState.AddModelError("Order", "Order already exists");
            return View();
        }

        FileStream fileStream = new FileStream(Path.Combine("/Users/resadsadiqov/RiderProjects/WebApplication/Pronia/wwwroot/assets/images/slider",vm.Photo.FileName),FileMode.Create);
        await  vm.Photo.CopyToAsync(fileStream);
        

        Slide slide = new Slide()
        {
            ShowSlide = vm.ShowSlide,
            Order = vm.Order,
            Title = vm.Title,
            Description = vm.Description,
            ImagePath = vm.Photo.FileName,
            Subtitle = vm.Subtitle,
            ButtonText = "Disvover now"
        };
        
        await _context.Slides.AddAsync(slide);
        
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
       
    }

    
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest();
        
        Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
        
        if (slide == null) return NotFound();
        
        System.IO.File.Delete(_environment.WebRootPath +"/assets/images/slider/" +slide.ImagePath);
        
        _context.Slides.Remove(slide);
        
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
        
    }
    
    
}