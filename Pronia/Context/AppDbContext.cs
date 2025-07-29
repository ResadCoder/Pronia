using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Context;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Category> Categories { get; set; } 
    
    public DbSet<ProductImage> ProductImages { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Slide> Slides { get; set; } 
    
    public DbSet<Card> Cards { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    
    public DbSet<Size> Sizes { get; set; }
    
    public DbSet<Color> Colors { get; set; }
    
    public DbSet<ProductColor> ProductColors { get; set; }
    
    public DbSet<ProductSize> ProductSizes { get; set; }
    
    public DbSet<Setting> Settings { get; set; }
    


}