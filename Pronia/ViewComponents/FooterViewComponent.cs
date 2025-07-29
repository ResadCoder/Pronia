using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;

namespace Pronia.ViewComponents;

public class FooterViewComponent(AppDbContext context): ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View(await context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value));
    }
}