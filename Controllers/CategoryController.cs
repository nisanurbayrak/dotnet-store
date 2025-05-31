using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class CategoryController : Controller
{

    private readonly DataContext _context;
    public CategoryController(DataContext context)
    {
        _context = context;
    }
    public ActionResult Index()
    {
        var categories = _context.Categories.Select(i => new CategoryGetModel
        {
            Id = i.Id,
            CategoryName = i.CategoryName,
            Url = i.Url, //buraya yazmazsan gösteremezsin
            ProductCount = i.ProductCategories.Count,
            IsActive = i.IsActive
        }).ToList();
        return View(categories);
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(CategoryCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = new Category
            {
                CategoryName = model.CategoryName,
                Url = model.Url,
                IsActive = model.IsActive
            };
            _context.Categories.Add(entity);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public ActionResult Edit(int id)
    {
        var entity = _context.Categories.Select(i => new CategoryEditModel
        {
            Id = i.Id,
            CategoryName = i.CategoryName,
            Url = i.Url,
            IsActive = i.IsActive
        }).FirstOrDefault(i => i.Id == id);
        if (entity == null)
        {
            return NotFound();
        }
        return View(entity);
    }
    [HttpPost]
    public ActionResult Edit(int id, CategoryEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            var entity = _context.Categories.FirstOrDefault(i => i.Id == model.Id);
            if (entity != null)
            {
                entity.CategoryName = model.CategoryName;
                entity.Url = model.Url;
                entity.IsActive = model.IsActive;

                _context.SaveChanges();
                TempData["Message"] = $"`{entity.CategoryName}` category updated successfully!";
                return RedirectToAction("Index");
            }

        }
        return View(model);
    }
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var entity = _context.Categories.FirstOrDefault(i => i.Id == id);
        if (entity != null)
        {
            return View(entity);
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "Geçersiz kategori ID.";
            return RedirectToAction("Index");
        }
        if (id == 1)
        {
            TempData["Message"] = "Bu Kategoriyi Silemezsiniz";
            return RedirectToAction("Index");
        }

        var category = _context.Categories
            .Include(c => c.ProductCategories)
            .FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            TempData["Error"] = "Kategori bulunamadı.";
            return RedirectToAction("Index");
        }

        if (category.Id == 1)
        {
            TempData["Error"] = "Bu kategori sistem kategorisidir ve silinemez.";
            return RedirectToAction("Index");
        }

        var productCategoryList = _context.ProductCategory
            .Where(pc => pc.CategoryId == category.Id)
            .ToList();

        foreach (var pc in productCategoryList)
        {
            pc.CategoryId = 1;
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();

        TempData["Message"] = $"\"{category.CategoryName}\" kategorisi silindi. Ürünler 'Silinmiş Kategori'ye taşındı.";
        return RedirectToAction("Index");
    }


}