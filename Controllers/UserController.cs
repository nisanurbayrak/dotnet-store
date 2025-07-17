using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class UserController : Controller
{
    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;
    public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<ActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var rolesPerUser = new Dictionary<int, IList<string>>();
        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            rolesPerUser[user.Id] = userRoles;
        }

        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
        ViewBag.RolesPerUser = rolesPerUser;

        return View(users);
    }

    public ActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create(UserCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Username,
                FirstName = model.Name,
                LastName = model.Surname,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    [HttpGet]
    public async Task<ActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

        return View(
            new UserEditModel
            {
                Username = user.UserName,
                Name = user.FirstName,
                Surname = user.LastName,
                Email = user.Email,
                SelectedRoles = await _userManager.GetRolesAsync(user)
            }
        );
    }
    [HttpPost]
    public async Task<ActionResult> Edit(string id, UserEditModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = model.Username;
            user.FirstName = model.Name;
            user.LastName = model.Surname;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Password) && model.Password == model.ConfirmPassword)
                {
                    await _userManager.RemovePasswordAsync(user);
                    var passwordResult = await _userManager.AddPasswordAsync(user, model.Password);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                if (model.SelectedRoles != null)
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
}