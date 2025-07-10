using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Context;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions options) : base(options){ }
    
    public DbSet<Category> Categories { get; set; } 
    
    public DbSet<ProductImage> ProductImages { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Slide> Slides { get; set; } 
    
    public DbSet<Card> Cards { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    
    public DbSet<Color> Colors { get; set; }
    
    public DbSet<ProductColor> ProductColors { get; set; }
}