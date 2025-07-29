using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Pronia.Areas.Manage.ViewModels;
using Pronia.Areas.Manage.ViewModels.Product;
using Pronia.Context;
using Pronia.Extensions;
using Pronia.Models;
using Pronia.Utilities;


namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]
public class ProductController : Controller
{
    private readonly AppDbContext _context ;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<IActionResult> Index()
    {
        List<ProductIndexVM> products = await _context.Products
            .Select(p => new ProductIndexVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                MainImage = p.ProductImages.FirstOrDefault(p => p.PositionEnum == ImagePositionEnum.main)!.ImgPath
            }).ToListAsync();
        
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        
        ProductCreateVM vm = new ProductCreateVM()
        {
           Categories = await _context.Categories.ToListAsync(),
           Sizes = await _context.Sizes.ToListAsync(),
           Colors = await _context.Colors.ToListAsync(),
        };
        
        return View(vm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        { 
            vm.Colors = await _context.Colors.ToListAsync();
            vm.Categories = await _context.Categories.ToListAsync();
            vm.Sizes = await _context.Sizes.ToListAsync();
            return View(vm);
        }

        if (await _context.Products.AnyAsync(p => p.Name == vm.Name))
        {
            vm.Colors = await _context.Colors.ToListAsync();
            vm.Categories = await _context.Categories.ToListAsync();
            vm.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError(nameof(vm.Name), "Product with same name already exists");
        }

        if (!await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
        {
            vm.Colors = await _context.Colors.ToListAsync();
            vm.Categories = await _context.Categories.ToListAsync();
            vm.Sizes = await _context.Sizes.ToListAsync();
            ModelState.AddModelError(nameof(vm.CategoryId), "Categories with same id already exists");
        }

        foreach (int colorId in vm.ColorIds)
        {
            if (!await _context.Colors.AnyAsync(c => c.Id == colorId))
            {
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Categories = await _context.Categories.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError(nameof(vm.ColorIds), "Color with this id doesn't exist");
            }
        }

        foreach (var sizeId in vm.SizeIds)
        {
            if (!await _context.Sizes.AnyAsync(s => s.Id == sizeId))
            {
                vm.Colors = await _context.Colors.ToListAsync();
                vm.Categories = await _context.Categories.ToListAsync();
                vm.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError(nameof(vm.SizeIds), "Size with this id doesn't exist");
            }
        }
        
        Product product = new Product
        {
            Name = vm.Name,
            Description = vm.Description,
            Price = vm.Price,
            Discount = vm.Discount,
            SKU = vm.Sku,
            CategoryId = vm.CategoryId,
            Colors = vm.ColorIds.Select(id => new ProductColor { ColorId = id }).ToList(),
            Sizes = vm.SizeIds.Select(id => new ProductSize { SizeId = id }).ToList()
        };
        
        product.ProductImages.Add(new ProductImage
        {
            PositionEnum = ImagePositionEnum.main,
            ImgPath = await vm.MainImage.CreateFileAsync(_webHostEnvironment.WebRootPath, "admin", "media", "products")
        });
        
        product.ProductImages.Add(new ProductImage
        {
            PositionEnum = ImagePositionEnum.hover,
            ImgPath =  await vm.HoverImage.CreateFileAsync(_webHostEnvironment.WebRootPath, "admin", "media", "products")
        });

        foreach (IFormFile add in vm.AdditionalImages)
        {
             product.ProductImages.Add(new ProductImage
             {
                 PositionEnum = ImagePositionEnum.additional,
                 ImgPath = await add.CreateFileAsync(_webHostEnvironment.WebRootPath, "admin", "media", "products")
             });
        }
        
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index)); 
    }

    public async Task<IActionResult> Update(int id)
    {
        if(id<=0) return BadRequest();
        Product? product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
            .Include(p => p.Sizes)
                .ThenInclude(ps => ps.Size)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        
        ProductUpdateVM vm = new ProductUpdateVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Discount = product.Discount,
            Images = product.ProductImages,
            Sku = product.SKU,
            Colors = await _context.Colors.ToListAsync(),
            Sizes = await _context.Sizes.ToListAsync(),
            Categories = await _context.Categories.ToListAsync(),
            CategoryId = product.CategoryId,
            ColorIds = product.Colors.Select(c => c.ColorId).ToList(),
            SizeIds = product.Sizes.Select(s => s.SizeId).ToList(),
            
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateVM vm)
    {
        if(id<=0) return BadRequest();
        
        Product? product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Colors)
                .ThenInclude(pc => pc.Color)
            .Include(p => p.Sizes)
                .ThenInclude(ps => ps.Size)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if(product==null) return NotFound();
        
        if (!ModelState.IsValid)
        {
            await GetRequiredDataAsync(vm);
            return View(vm);
        }

        if (!string.Equals(product.Name, vm.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            if (await _context.Products.AnyAsync(p => p.Name == vm.Name))
            {
                await GetRequiredDataAsync(vm);
                ModelState.AddModelError(nameof(vm.Name), "Product with same name already exists");
                return View(vm);
            }
        }
        
        if (vm.CategoryId != product.CategoryId  && !await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
        {
                await GetRequiredDataAsync(vm);
                ModelState.AddModelError(nameof(vm.CategoryId), "Category with same id doesn't exist");
                return View(vm);
        }

        foreach (ProductColor pc in product.Colors)
        {
            if (!vm.ColorIds.Contains((pc.ColorId)))
            {
                _context.ProductColors.Remove(pc);
            }
        }
        // product.Colors = product.Colors.Where(pc => vm.ColorIds.Contains(pc.ColorId)).ToList();
        
        foreach (int colorId in vm.ColorIds)
        {
            if (!product.Colors.Any(c => c.ColorId == colorId))
            {
                if (!await _context.Colors.AnyAsync(c => c.Id == colorId))
                {
                    await GetRequiredDataAsync(vm);
                    ModelState.AddModelError(nameof(colorId), "Color with same id doesn't exist");
                    return View(vm);
                }
                product.Colors.Add(new ProductColor { ColorId = colorId});
            }
        }

        foreach (ProductSize ps in product.Sizes)
        {
            if (!vm.SizeIds.Contains((ps.SizeId)))
            {
                _context.ProductSizes.Remove(ps);
            }
        }
        
        foreach (int sizeId in vm.SizeIds)
        {
            if (!product.Sizes.Any(s => s.SizeId == sizeId))
            {
                if (!await _context.Sizes.AnyAsync(s => s.Id == sizeId))
                {
                    await GetRequiredDataAsync(vm);
                    ModelState.AddModelError(nameof(sizeId), "Size with same id doesn't exist");
                    return View(vm);
                }
                product.Sizes.Add(new ProductSize { SizeId = sizeId });
            }
        }

        if (vm.MainImage != null)
        {
           ProductImage mainImg = product.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.main)!;
           mainImg.ImgPath.DeleteFile(_webHostEnvironment.WebRootPath, "admin", "media", "products");
           
           mainImg.ImgPath = await vm.MainImage.CreateFileAsync(_webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        
        if (vm.HoverImage != null)
        {
             ProductImage  hoverImg = product.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.hover)!;
             hoverImg.ImgPath.DeleteFile(_webHostEnvironment.WebRootPath, "admin", "media", "products");
             hoverImg.ImgPath = await vm.HoverImage.CreateFileAsync(_webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        
        product.Name = vm.Name;
        product.Description = vm.Description;
        product.Price = vm.Price;
        product.Discount = vm.Discount;
        product.SKU = vm.Sku;
        product.CategoryId = vm.CategoryId;
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if(id<=0) return BadRequest();
        Product? product = await _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);
        if(product==null) return NotFound();
        foreach (var img in product.ProductImages)
        {
            img.ImgPath.DeleteFile(_webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return BadRequest();
        
        ProductDetailsVM? vm = await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsVM
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Sku = p.SKU,

                CategoryName = p.Category.Name,  
                Sizes = p.Sizes.Select(s => s.Size.Name).ToList(),
                Colors = p.Colors.Select(c => c.Color.Name).ToList(),

                MainImagePath = p.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.main)!.ImgPath,
                HoverImagePath = p.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.hover)!.ImgPath,
                AdditionalImages = p.ProductImages
                    .Where(pi => pi.PositionEnum == ImagePositionEnum.additional)
                    .Select(pi => pi.ImgPath)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        
        return View(vm);
    }
    
    
    
    private async Task GetRequiredDataAsync(ProductUpdateVM vm)
    {
        vm.Colors = await _context.Colors.ToListAsync();
        vm.Categories = await _context.Categories.ToListAsync();
        vm.Sizes = await _context.Sizes.ToListAsync();
        vm.Images = await _context.ProductImages.ToListAsync();
    }
}

