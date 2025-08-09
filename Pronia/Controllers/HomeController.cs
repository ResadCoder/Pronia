using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.DTO;
using Pronia.Models;
using Pronia.Utilities;
using Pronia.ViewModels;
using Pronia.ViewModels.Home;
namespace Pronia.Controllers;


public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var slides = await _context.Slides
            .Where(s => s.ShowSlide)
            .OrderBy(s => s.Order)
            .ToListAsync();
        var cards = await _context.Cards.ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var reviews = await GetReviewsAsync();
        var latestBlogs = await GetLatestBlogsAsync();
        var products = await GetFeaturedProductsAsync();
        

        var model = new HomeIndexViewModel
        {
            Slides = slides,
            Cards = cards,
            Reviews = reviews,
            Blogs = latestBlogs,
            Products = products,
            Categories = categories 
        };

        return View(model);
    }

   

    private async Task<List<ReviewViewModel>> GetReviewsAsync()
    {
        return await _context.Reviews
            .Select(r => new ReviewViewModel
            {
                Commentary = r.Commentary,
                UserName = r.UserName,
                UserImagePath = r.UserImagePath
                
            })
            .ToListAsync();
    }

    private async Task<List<BlogViewModel>> GetLatestBlogsAsync()
    {
        return await _context.Blogs
            .Select(b => new BlogViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Comment = b.Comment,
                ImgPath = b.ImgPath,
                Date = b.Date,
                CreatedBy = b.CreatedBy
               
            })
            .ToListAsync();
    }

    private async Task<List<HomeIndexProductVM>> GetFeaturedProductsAsync()
    {
        return await _context.Products
            .Include(p => p.ProductImages)
            .Select(p => new HomeIndexProductVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Images = p.ProductImages.Select(i => new ProductImageViewModel
                {
                    ImgPath = i.ImgPath,
                    PositionEnum = i.PositionEnum
                }).ToList()
                
            })
            .ToListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> GetQuickView(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Colors).ThenInclude(pc => pc.Color)
            .Include(p => p.Sizes).ThenInclude(ps => ps.Size)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var vm = new QuickViewProductVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            MainImage = product.ProductImages
                .FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.main)?.ImgPath ?? "",
            Colors = product.Colors.Select(c => new ColorVM
            {
                Id = c.Color.Id,
                Name = c.Color.Name
            }).ToList(),
            Sizes = product.Sizes.Select(s => new SizeVM
            {
                Id = s.Size.Id,
                Name = s.Size.Name
            }).ToList()
        };

        var colorCount = vm.Colors.Count;
        var sizeCount = vm.Sizes.Count;
        
        return Json(vm);
    }
}

