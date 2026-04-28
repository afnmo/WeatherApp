using WeatherApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Controllers;

public class AuthController : Controller
{

    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)    {
        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            ViewBag.Error = "Username and password are required";
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Weather");
        }
        
        ViewBag.Error = "invalid login";
        return View();
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

}