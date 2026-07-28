using System.Text;
using LMSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace LMSystem.Controllers
{
    // Houses the Identity account flows the old hardcoded login never had:
    // self-registration, forgot/reset password, change password, and a real
    // Access Denied page. Built as plain MVC (matching the rest of this app)
    // rather than the Razor Pages Identity scaffolding, since this project
    // doesn't use Razor Pages anywhere else.
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ---------------------------------------------------------------
        // Register
        // ---------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByNameAsync(model.Username!);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "That username is already taken.");
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true // no email service is configured, so treat as confirmed
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Self-registration is intentionally limited to the Member role.
            // Administrator/Librarian accounts are created by staff (via SeedData
            // today; a future "manage users" screen could extend this).
            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["message"] = "Account created - welcome!";
            return RedirectToAction("Index", "Dashboard");
        }

        // ---------------------------------------------------------------
        // Forgot password / Reset password
        // ---------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                // Don't reveal whether the account exists - show the same
                // confirmation page either way.
                return View("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = Url.Action(nameof(ResetPassword), "Account",
                new { email = user.Email, token = encodedToken }, Request.Scheme);

            // No SMTP/email service is wired up in this project, so the reset
            // link is shown directly on screen instead of emailed. Swap this
            // for a real IEmailSender call if one gets configured later.
            ViewBag.DevResetLink = resetLink;
            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                // Don't reveal whether the account exists.
                return View("ResetPasswordConfirmation");
            }

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token!));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The reset link is invalid or has expired.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return View("ResetPasswordConfirmation");
        }

        // ---------------------------------------------------------------
        // Change password (for an already logged-in user)
        // ---------------------------------------------------------------
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Keeps the current session valid after the security stamp changes.
            await _signInManager.RefreshSignInAsync(user);
            TempData["message"] = "Your password has been changed.";
            return RedirectToAction("Index", "Dashboard");
        }

        // ---------------------------------------------------------------
        // Access denied - shown when a logged-in user hits a page their
        // role doesn't allow (as opposed to not being logged in at all).
        // ---------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
