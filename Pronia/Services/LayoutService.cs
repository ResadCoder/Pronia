using Microsoft.EntityFrameworkCore;
using Pronia.Context;

namespace Pronia.Services;

public class LayoutService(AppDbContext context)
{
    public async Task<Dictionary<string, string>> GetSettingsAsync()
    {
        return await context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
    }
}