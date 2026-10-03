using MaleFashion.Application.Contracts.Services;
using MaleFashion.Domain.Constants;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

public class AccountController : Controller
{
    private const string RecaptchaFailedMessage = "reCAPTCHA verification failed. Please try again.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IRecaptchaService _recaptchaService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        IRecaptchaService recaptchaService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _recaptchaService = recaptchaService;
    }

    // ---------------- Register ----------------

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // reCAPTCHA v3 (action name must match grecaptcha.execute(..., { action: 'register' }))
        if (!await _recaptchaService.VerifyAsync(model.RecaptchaToken, "register"))
        {
            ModelState.AddModelError(string.Empty, RecaptchaFailedMessage);
            return View(model);
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "An account with this email already exists.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, RoleNames.Customer);
        await _signInManager.SignInAsync(user, isPersistent: false);

        TempData["SignupSuccess"] = "Your account has been created successfully.";
        return RedirectToAction("Index", "Home");
    }

    // ---------------- Login ----------------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // reCAPTCHA v3
        if (!await _recaptchaService.VerifyAsync(model.RecaptchaToken, "login"))
        {
            ModelState.AddModelError(string.Empty, RecaptchaFailedMessage);
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    // ---------------- Forgot / Reset password ----------------

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        // Always show the same confirmation, whether or not the email exists,
        // to avoid leaking which emails are registered.
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword", "Account",
                new { email = user.Email, token },
                protocol: Request.Scheme);

            var htmlMessage = $"""
                <p>You requested a password reset for your MaleFashion account.</p>
                <p><a href="{resetLink}">Click here to reset your password</a></p>
                <p>If you did not request this, you can safely ignore this email.</p>
                """;

            await _emailService.SendEmailAsync(user.Email!, "Reset your MaleFashion password", htmlMessage);
        }

        return View("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token = null, string? email = null)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return BadRequest("A valid password reset token and email are required.");
        }

        return View(new ResetPasswordViewModel { Token = token, Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Same "always succeed" messaging to avoid revealing account existence.
            return View("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

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

    // ---------------- Change password ----------------

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [Authorize]
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
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["ChangePasswordSuccess"] = "Your password has been changed successfully.";
        return RedirectToAction(nameof(ChangePassword));
    }

    // ---------------- Logout ----------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        Response.Cookies.Delete("MaleFashion.GuestCartId");
        return RedirectToAction("Index", "Home");
    }
}
