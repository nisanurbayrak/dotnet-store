using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using dotnet_store.Services;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;



namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    private UserManager<IdentityUser> _userManager;
    private readonly EmailSender _emailSender;
    private readonly DataContext _db;

    public AccountController(UserManager<IdentityUser> userManager, EmailSender emailSender, DataContext db)
    {
        _db = db;
        _userManager = userManager;
        _emailSender = emailSender;
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
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };

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


}
// "SendGrid": {
//   "ApiKey": "O0n7kgBLShqH5mmKu8mAeA",
//   "FromEmail": "fordotnetstoreproject@gmail.com",
//   "FromName": "dotnet store"
// }
