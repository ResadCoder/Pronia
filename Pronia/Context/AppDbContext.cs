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
    
    public DbSet<Size> Sizes { get; set; }
    
    public DbSet<Color> Colors { get; set; }
    
    public DbSet<ProductColor> ProductColors { get; set; }
    
    public DbSet<ProductSize> ProductSizes { get; set; }
    
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     // ProductColor - Product əlaqəsi (cascade)
    //     modelBuilder.Entity<ProductColor>()
    //         .HasOne(pc => pc.Product)
    //         .WithMany(p => p.ProductColors)
    //         .HasForeignKey(pc => pc.ProductId)
    //         .OnDelete(DeleteBehavior.Cascade); // Yəni product silinsə, əlaqələri sil
    //
    //     // ProductColor - Color əlaqəsi (cascade)
    //     modelBuilder.Entity<ProductColor>()
    //         .HasOne(pc => pc.Color)
    //         .WithMany(c => c.ProductColors)
    //         .HasForeignKey(pc => pc.ColorId)
    //         .OnDelete(DeleteBehavior.Cascade); // Yəni color silinsə, əlaqələri sil
    //
    //     base.OnModelCreating(modelBuilder);
    // }
}