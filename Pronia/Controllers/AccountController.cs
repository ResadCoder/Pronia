using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Extensions;
using Pronia.Models;
using Pronia.Utilities.Interfaces;
using Pronia.ViewModels;

namespace Pronia.Controllers;

public class AccountController(UserManager<User> usermanager, SignInManager<User> signinmanager,RoleManager<IdentityRole<int>> roleManager,IEmailService emailService,IViewRenderService viewRenderService ) : Controller
{
    // GET
    public IActionResult Register()
    {
        if (User.Identity!.IsAuthenticated) return NotFound();
        return View();
    }
    
    [HttpPost]

    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        User newUser = new User
        {
            UserName = vm.Username,
            Email = vm.Email,
            Name = vm.Name,
            Surname = vm.Surname
        };
        IdentityResult result = await usermanager.CreateAsync(newUser, vm.Password);

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
                
                string token = await usermanager.GenerateEmailConfirmationTokenAsync(newUser);
                string link = Url.Action(nameof(ConfirmEmail), "Account", new {Token = token, Email = vm.Email },
                     Request.Scheme)!;
                string body = await viewRenderService.RenderToStringAsync("EmailTemplate/ConfirmEmail", link);
                await emailService.SendEmailAsync(vm.Email, "Email Confirmation", body);
                break;
        }
        
        ViewBag.ConfirmEmail = true;
        return RedirectToAction(nameof(Login)); 
    }

    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        User? user = await usermanager.FindByEmailAsync(email);
        if (user == null) return NotFound();
        IdentityResult result = await usermanager.ConfirmEmailAsync(user, token);
        if(!result.Succeeded) return NotFound();
        
        await signinmanager.SignInAsync(user, false);
        
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
    
    public async Task<IActionResult> Logout()
    {
        await signinmanager.SignOutAsync();
        return RedirectToAction(nameof(Login));
        
    }

    public IActionResult Login()
    {
        if (User.Identity!.IsAuthenticated) return NotFound();
        return View();
    }

    [HttpPost]

    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl)
    {
        if(!ModelState.IsValid) return View(vm);

        var user = await usermanager.FindByNameAsync(vm.UsernameOrEmail);

        if (user is null)
        {
            user = await usermanager.FindByEmailAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email/Username or Password is invalid.");
                return View(vm);
            }
        }
        
        var result = await signinmanager.CheckPasswordSignInAsync(user, vm.Password, true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
                return View(vm);
            }

            ModelState.AddModelError(string.Empty, "Email/Username or Password is invalid.");
            return View(vm);
        }
        
        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Email/Username or Password is invalid. ");
        }
        
        await signinmanager.SignInAsync(user, vm.RememberMe );
        
        if (returnUrl != null) 
            return Redirect(returnUrl);
        
        
        return RedirectToAction("Index","Home");
    }

    public async Task<IActionResult> CreateRoles()
    {
        foreach (UserRoleEnum  role in  Enum.GetValues<UserRoleEnum>())
        {
            if(!await roleManager.RoleExistsAsync(role.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role.ToString()));
            }
        }
        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> MyAccount()
    {
        return View();
    }
}
