using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Controllers
{
    // Public - this is the one controller that must stay reachable without
    // already being logged in, despite the global AuthorizeFilter in Program.cs.
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(LoginModel usr)
        {
            if (string.IsNullOrWhiteSpace(usr.username) || string.IsNullOrWhiteSpace(usr.password))
            {
                ViewBag.message = "Login Failed";
                return View("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName: usr.username,
                password: usr.password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["message"] = "Login Success";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.message = "Login Failed";
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
