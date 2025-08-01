namespace Pronia.Utilities.Interfaces;

public interface IViewRenderService
{
    Task<string> RenderToStringAsync(string viewName, object model);
}