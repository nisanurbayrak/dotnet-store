using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using dotnet_store.Services;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager;
    private readonly EmailSender _emailSender;
    private readonly DataContext _db;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailSender emailSender, DataContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _db = db;
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser { UserName = model.Username, Email = model.Email, FirstName = model.Name, LastName = model.Surname };

            var result = await _userManager.CreateAsync(user, model.Password);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            // if (result.Succeeded)
            // {
            //     return RedirectToAction("Index", "Home");
            // }

            var confirmationLink = Url.Action("Confirm", "Account", new { Email = model.Email }, Request.Scheme);
            await _emailSender.SendConfirmationEmail(model.Email, confirmationLink);
            ViewBag.Message = "Onay maili gönderildi! Lütfen emailinizi kontrol edin.";
            return View();
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Confirm(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.Message = "Geçersiz istek.";
            return View();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            ViewBag.Message = "Kullanıcı bulunamadı.";
            return View();
        }

        if (user.EmailConfirmed)
        {
            ViewBag.Message = "Hesap zaten onaylanmış.";
            return View();
        }

        // Onaylama işlemi
        user.EmailConfirmed = true;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        ViewBag.Message = "Hesabınız başarıyla onaylandı!";
        return View();
    }

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    var lockOutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeRemaining = lockOutDate.Value - DateTimeOffset.UtcNow;
                    ModelState.AddModelError("", $"Hesabınız kilitlenmiş. Lütfen {timeRemaining.Minutes + 1} dakika sonra tekrar deneyin.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Bu kullanıcı hesabı giriş yapmaya izin verilmiyor.");
                }
                else if (result.RequiresTwoFactor)
                {

                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz email veya şifre.");
                }
            }
            else if (user != null && !user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Email onaylanmamış. Lütfen emailinizi kontrol edin.");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı veya email onaylanmamış.");
            }
        }
        return View(model);
    }

    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
    [Authorize]
    public ActionResult Settings()
    {
        return View();
    }

}

