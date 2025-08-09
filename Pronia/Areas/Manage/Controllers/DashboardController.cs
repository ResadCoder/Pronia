using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.Manage.Controllers;

    [Area("Manage")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}