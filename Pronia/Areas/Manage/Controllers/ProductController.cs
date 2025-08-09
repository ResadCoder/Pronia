using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [AutoValidateAntiforgeryToken]
public class ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        List<ProductIndexVM> products = await context.Products
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
           Categories = await context.Categories.ToListAsync(),
           Sizes = await context.Sizes.ToListAsync(),
           Colors = await context.Colors.ToListAsync(),
        };
        
        return View(vm);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        { 
            vm.Colors = await context.Colors.ToListAsync();
            vm.Categories = await context.Categories.ToListAsync();
            vm.Sizes = await context.Sizes.ToListAsync();
            return View(vm);
        }

        if (await context.Products.AnyAsync(p => p.Name == vm.Name))
        {
            vm.Colors = await context.Colors.ToListAsync();
            vm.Categories = await context.Categories.ToListAsync();
            vm.Sizes = await context.Sizes.ToListAsync();
            ModelState.AddModelError(nameof(vm.Name), "Product with same name already exists");
        }

        if (!await context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
        {
            vm.Colors = await context.Colors.ToListAsync();
            vm.Categories = await context.Categories.ToListAsync();
            vm.Sizes = await context.Sizes.ToListAsync();
            ModelState.AddModelError(nameof(vm.CategoryId), "Categories with same id already exists");
        }

        foreach (int colorId in vm.ColorIds)
        {
            if (!await context.Colors.AnyAsync(c => c.Id == colorId))
            {
                vm.Colors = await context.Colors.ToListAsync();
                vm.Categories = await context.Categories.ToListAsync();
                vm.Sizes = await context.Sizes.ToListAsync();
                ModelState.AddModelError(nameof(vm.ColorIds), "Color with this id doesn't exist");
            }
        }

        foreach (var sizeId in vm.SizeIds)
        {
            if (!await context.Sizes.AnyAsync(s => s.Id == sizeId))
            {
                vm.Colors = await context.Colors.ToListAsync();
                vm.Categories = await context.Categories.ToListAsync();
                vm.Sizes = await context.Sizes.ToListAsync();
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
            ImgPath = await vm.MainImage.CreateFileAsync(webHostEnvironment.WebRootPath, "admin", "media", "products")
        });
        
        product.ProductImages.Add(new ProductImage
        {
            PositionEnum = ImagePositionEnum.hover,
            ImgPath =  await vm.HoverImage.CreateFileAsync(webHostEnvironment.WebRootPath, "admin", "media", "products")
        });

        foreach (IFormFile add in vm.AdditionalImages)
        {
             product.ProductImages.Add(new ProductImage
             {
                 PositionEnum = ImagePositionEnum.additional,
                 ImgPath = await add.CreateFileAsync(webHostEnvironment.WebRootPath, "admin", "media", "products")
             });
        }
        
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index)); 
    }

    public async Task<IActionResult> Update(int id)
    {
        if(id<=0) return BadRequest();
        Product? product = await context.Products
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
            Colors = await context.Colors.ToListAsync(),
            Sizes = await context.Sizes.ToListAsync(),
            Categories = await context.Categories.ToListAsync(),
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
        
        Product? product = await context.Products
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
            if (await context.Products.AnyAsync(p => p.Name == vm.Name))
            {
                await GetRequiredDataAsync(vm);
                ModelState.AddModelError(nameof(vm.Name), "Product with same name already exists");
                return View(vm);
            }
        }
        
        if (vm.CategoryId != product.CategoryId  && !await context.Categories.AnyAsync(c => c.Id == vm.CategoryId))
        {
                await GetRequiredDataAsync(vm);
                ModelState.AddModelError(nameof(vm.CategoryId), "Category with same id doesn't exist");
                return View(vm);
        }

        foreach (ProductColor pc in product.Colors)
        {
            if (!vm.ColorIds.Contains((pc.ColorId)))
            {
                context.ProductColors.Remove(pc);
            }
        }
        // product.Colors = product.Colors.Where(pc => vm.ColorIds.Contains(pc.ColorId)).ToList();
        
        foreach (int colorId in vm.ColorIds)
        {
            if (!product.Colors.Any(c => c.ColorId == colorId))
            {
                if (!await context.Colors.AnyAsync(c => c.Id == colorId))
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
                context.ProductSizes.Remove(ps);
            }
        }
        
        foreach (int sizeId in vm.SizeIds)
        {
            if (!product.Sizes.Any(s => s.SizeId == sizeId))
            {
                if (!await context.Sizes.AnyAsync(s => s.Id == sizeId))
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
           mainImg.ImgPath.DeleteFile(webHostEnvironment.WebRootPath, "admin", "media", "products");
           
           mainImg.ImgPath = await vm.MainImage.CreateFileAsync(webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        
        if (vm.HoverImage != null)
        {
             ProductImage  hoverImg = product.ProductImages.FirstOrDefault(pi => pi.PositionEnum == ImagePositionEnum.hover)!;
             hoverImg.ImgPath.DeleteFile(webHostEnvironment.WebRootPath, "admin", "media", "products");
             hoverImg.ImgPath = await vm.HoverImage.CreateFileAsync(webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        
        product.Name = vm.Name;
        product.Description = vm.Description;
        product.Price = vm.Price;
        product.Discount = vm.Discount;
        product.SKU = vm.Sku;
        product.CategoryId = vm.CategoryId;
        
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if(id<=0) return BadRequest();
        Product? product = await context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);
        if(product==null) return NotFound();
        foreach (var img in product.ProductImages)
        {
            img.ImgPath.DeleteFile(webHostEnvironment.WebRootPath, "admin", "media", "products");
        }
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return BadRequest();
        
        ProductDetailsVM? vm = await context.Products
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
        vm.Colors = await context.Colors.ToListAsync();
        vm.Categories = await context.Categories.ToListAsync();
        vm.Sizes = await context.Sizes.ToListAsync();
        vm.Images = await context.ProductImages.ToListAsync();
    }
}

