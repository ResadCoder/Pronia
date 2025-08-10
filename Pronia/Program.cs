using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Models;
using Pronia.Services;
using Pronia.Utilities.Interfaces;

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
        });

        builder.Services.AddScoped<LayoutService>();
        builder.Services.AddScoped<AppDbContextInitializer>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
       

        // Identity
        builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            AppDbContextInitializer initializer  = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
            initializer.InitializeAsync().Wait();
        }

        app.MapControllerRoute(
            name: "manage",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}