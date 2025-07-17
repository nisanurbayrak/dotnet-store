using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace dotnet_store.Controllers;
public class RoleController : Controller
{

    private RoleManager<AppRole> _roleManager;
    private readonly DataContext _context;

    public RoleController(RoleManager<AppRole> roleManager, DataContext context)
    {
        _context = context;
        _roleManager = roleManager;
    }
    public ActionResult Index()
    {
        return View(_roleManager.Roles);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(RoleCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var role = new AppRole { Name = model.RoleName };
            var result = await _roleManager.CreateAsync(role);

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


    public async Task<ActionResult> Edit(string id)
    {
        var entity = await _roleManager.FindByIdAsync(id);

        if (entity != null)
        {
            return View(new RoleEditModel { RoleName = entity.Name! });
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> Edit(RoleEditModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = await _roleManager.FindByIdAsync(model.Id);

            if (entity != null)
            {
                entity.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(entity);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    public ActionResult DeleteConfirm(string? id)
    {
        if (id == null)
        {
            TempData["Error"] = "Geçersiz kategori ID.";
            return RedirectToAction("Index");
        }

        var role = _context.Roles.FirstOrDefault(i => i.Name == id);

        if (role == null)
        {
            TempData["Error"] = "Rol bulunamadı.";
            return RedirectToAction("Index");
        }

        if (role.Name == "Admin")
        {
            TempData["Error"] = "Bu kategori sistem rolüdür ve silinemez.";
            return RedirectToAction("Index");
        }

        _context.Roles.Remove(role);
        _context.SaveChanges();

        TempData["Message"] = $"\"{role.Name}\" rolü silindi";
        return RedirectToAction("Index");
    }


}