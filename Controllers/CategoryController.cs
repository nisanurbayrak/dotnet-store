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
            return RedirectToAction("Index");
        }
        var entity = _context.Categories
            .Include(c => c.ProductCategories)
            .ThenInclude(c => c.Product)
            .FirstOrDefault(c => c.Id == id);

        var relatedProducts = entity.ProductCategories.Select(pc => pc.Product).ToList();

        if (relatedProducts.Count > 0 && entity.IsActive == true)
        {
            foreach (var product in relatedProducts)
            {
                product.Category.Id = 1;
            }
            entity.IsActive = false;
            TempData["Message"] = $"`{entity.CategoryName}` kategorisi silinemez. Bu kategoriye ait ürünler var. Kategori durumu pasif yapılabilir.";
            entity.IsActive = false;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        if (entity != null && (relatedProducts.Count == 0 || entity.IsActive == false))
        {
            _context.Categories.Remove(entity);
            _context.SaveChanges();
            TempData["Message"] = $"`{entity.CategoryName}` category deleted successfully!";
        }

        return RedirectToAction("Index");
    }
}