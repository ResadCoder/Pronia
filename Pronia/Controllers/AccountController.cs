using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers;

public class AccountController(UserManager<User> usermanager, SignInManager<User> signinmanager)
    : Controller
{
    private readonly UserManager<User> _userManager = usermanager;
    private readonly SignInManager<User> _signInManager = signinmanager;

    // GET
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]

    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        User? newUser = new User
        {
            UserName = vm.Username,
            Email = vm.Email,
            Name = vm.Name,
            Surname = vm.Surname
        };
        IdentityResult result = await _userManager.CreateAsync(newUser, vm.Password);

        switch (result.Succeeded)
        {
            case false:
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(vm);
            }
            case true:
                await _signInManager.SignInAsync(newUser, false);
                break;
        }
        return RedirectToAction("Index","Home");
    }
}