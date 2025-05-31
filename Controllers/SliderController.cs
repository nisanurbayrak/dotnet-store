using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class SliderController : Controller
{
    private readonly DataContext _context;
    public SliderController(DataContext context)
    {
        _context = context;
    }
    public ActionResult Index()
    {
        return View(_context.Sliders.Select(i => new SliderGetModel
        {
            Id = i.Id,
            Title = i.Title,
            Active = i.Active,
            Index = i.Index,
            Image = i.Image
        }).ToList());
    }
    public ActionResult Create()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Create(SliderCreateModel model)
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

        if (ModelState.IsValid)
        {
            var fileName = model.Image != null
                ? Path.GetRandomFileName() + ".jpg"
                : "pho.png";

            if (model.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }

            foreach (var item in _context.Sliders)
            {
                if (item.Index == model.Index && item.Id != model.Id)
                {
                    item.Index++;
                }
            }

            var slider = new Slider
            {
                Title = model.Title,
                Description = model.Description,
                Active = model.Active,
                Index = model.Index,
                Image = fileName
            };
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        return View();
    }

    public IActionResult Edit(int id)
    {
        var slider = _context.Sliders.FirstOrDefault(i => i.Id == id);
        if (slider == null)
        {
            return NotFound();
        }

        int newIndex = slider.Index;
        var conflictingSliders = _context.Sliders
        .Where(i => i.Id != id && i.Index >= newIndex)
        .OrderBy(i => i.Index)
        .ToList();

        var categories = _context.Categories.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");


        var model = new SliderEditModel
        {
            Id = slider.Id,
            Title = slider.Title,
            Description = slider.Description,
            Active = slider.Active,

            Index = slider.Index,
            Image = slider.Image
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(SliderEditModel model)
    {

        if (!ModelState.IsValid)
        {
            var oldEntity = _context.Sliders.FirstOrDefault(p => p.Id == model.Id);
            if (oldEntity != null && string.IsNullOrEmpty(model.Image))
            {
                model.Image = oldEntity.Image;
            }
            return View(model);
        }

        if (model.ImageFile != null)
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(model.ImageFile.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            model.Image = fileName;
        }

        var slider = _context.Sliders.FirstOrDefault(i => i.Id == model.Id);


        foreach (var item in _context.Sliders)
        {
            if (item.Index == model.Index && item.Id != model.Id)
            {
                item.Index++;
            }
        }

        slider.Title = model.Title;
        slider.Description = model.Description;
        slider.Active = model.Active;
        slider.Index = model.Index;
        slider.Image = model.Image;

        _context.SaveChangesAsync();

        TempData["Message"] = $"`{slider.Title}` updated successfully!";
        return RedirectToAction("Index", "Slider");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
        {
            return NotFound();
        }
        _context.Sliders.Remove(slider);
        _context.SaveChanges();
        TempData["Message"] = $"`{slider.Title}` deleted successfully!";
        return RedirectToAction("Index");
    }
}