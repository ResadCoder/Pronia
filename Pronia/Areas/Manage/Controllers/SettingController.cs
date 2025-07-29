using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Manage.Controllers;

        [Area("Manage")]
    public class SettingController(AppDbContext context) : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            ICollection<Setting> settings = await context.Settings.ToListAsync();
            return View(settings);
        }

        public async Task<IActionResult> Update(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest();
            Setting? setting = await context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null) return
                NotFound();
            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string key, Setting setting)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest();
            Setting? existingSetting = await context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            if (existingSetting == null)
                return NotFound();
            
            existingSetting.Value = setting.Value.Trim();
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
}