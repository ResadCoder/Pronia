using Microsoft.EntityFrameworkCore;
using Pronia.Context;

namespace Pronia;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
                }
            );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAuthorization();
        
        app.UseStaticFiles();
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        
        app.MapControllerRoute(
            name: "manage",
            pattern: "{controller=Home}/{action=Index}/{id?}");
      
        

        app.Run(); 
    }
}