using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace dotnet_store.Controllers;

public class ProductController : Controller
{
    private readonly DataContext _context;

    public ProductController(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResult> Index(int? category)
    {
        var productsQuery = _context.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .Where(p => p.IsActive);

        if (category.HasValue)
        {
            productsQuery = productsQuery
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == category.Value));
        }

        var products = await productsQuery
            .Select(p => new ProductGetModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price ?? 0,
                IsActive = p.IsActive,
                IsHome = p.IsHome,
                CategoryName = p.ProductCategories.FirstOrDefault().Category.CategoryName ?? "No Category",
                Stock = p.Stock,
                Image = p.Image
            })
            .ToListAsync();

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "CategoryName", category);

        string selectedCategoryName = "All Categories";
        if (category.HasValue)
        {
            selectedCategoryName = await _context.Categories
                .Where(c => c.Id == category.Value)
                .Select(c => c.CategoryName)
                .FirstOrDefaultAsync() ?? "Unknown Category";
        }

        ViewBag.SelectedCategoryName = selectedCategoryName;

        return View(products);
    }

    public ActionResult List(string url, string q)
    {
        var query = _context.Products
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(url))
        {
            query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Url == url));
        }

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(q.ToLower()));
            ViewData["q"] = q;
        }

        return View(query.ToList());
    }
    public ActionResult Details(int id)
    {
        var product = _context.Products
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var firstCategoryId = product.ProductCategories.FirstOrDefault()?.CategoryId;

        if (firstCategoryId != null)
        {
            ViewData["BenzerUrunler"] = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.IsActive && p.ProductCategories.Any(pc => pc.CategoryId == firstCategoryId && p.Id != id))
                .Take(4)
                .ToList();
        }

        return View(product);
    }

    [HttpGet("product/create")]
    public ActionResult Create()
    {
        ViewBag.Categories = new MultiSelectList(_context.Categories.ToList(), "Id", "CategoryName");
        return View();
    }

    [HttpPost("product/create")]
    public async Task<ActionResult> Create(ProductCreateModel model)
    {
        ViewBag.Categories = new MultiSelectList(_context.Categories.ToList(), "Id", "CategoryName", model.CategoryIds);

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

            if (model.Stock == 0)
            {
                model.IsActive = false;
                model.IsHome = false;
            }

            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                IsActive = model.IsActive,
                IsHome = model.IsHome,
                Image = fileName,
                Stock = model.Stock ?? 0,
                ProductCategories = model.CategoryIds?.Select(cid => new ProductCategory
                {
                    CategoryId = cid
                }).ToList()
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }

        return View(model);
    }


    [HttpGet]
    public ActionResult Edit(int id)
    {
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");

        var entity = _context.Products
        .Include(p => p.ProductCategories)
        .Select(i => new ProductEditModel
        {
            Id = i.Id,
            ProductName = i.ProductName,
            Price = i.Price ?? 0,
            IsActive = i.IsActive,
            Image = i.Image,
            IsHome = i.IsHome,
            Stock = i.Stock,
            CategoryIds = i.ProductCategories.Select(pc => pc.CategoryId).ToList(),
            Description = i.Description
        })
        .FirstOrDefault(i => i.Id == id);

        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.SelectedCategoryIds = entity.CategoryIds;
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "CategoryName");

        if (model.Stock == 0)
        {
            model.IsActive = false;
            model.IsHome = false;
        }

        if (!ModelState.IsValid)
        {
            var oldEntity = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (oldEntity != null && string.IsNullOrEmpty(model.Image))
            {
                model.Image = oldEntity.Image;
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Hata: {error.ErrorMessage}");
            }
            return View(model);
        }

        var entity = await _context.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (entity == null)
        {
            return RedirectToAction("Index", "Product");
        }

        if (model.ImageFile != null)
        {
            var fileName = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            model.Image = fileName;
        }
        else
        {
            model.Image = model.Image ?? entity.Image ?? "pho.png";
        }

        entity.ProductName = model.ProductName;
        entity.Price = model.Price ?? 0;
        entity.IsActive = model.IsActive;
        entity.IsHome = model.IsHome;
        entity.Image = model.Image;
        entity.Stock = model.Stock ?? 0;
        entity.Description = model.Description;

        var existingCategories = _context.ProductCategory.Where(pc => pc.ProductId == entity.Id);
        _context.ProductCategory.RemoveRange(existingCategories);

        if (model.CategoryIds != null && model.CategoryIds.Any())
        {
            foreach (var categoryId in model.CategoryIds.Distinct())
            {
                _context.ProductCategory.Add(new ProductCategory
                {
                    ProductId = entity.Id,
                    CategoryId = categoryId
                });
            }
        }
        await _context.SaveChangesAsync();

        TempData["Message"] = $"{entity.ProductName} updated successfully!";
        return Redirect("/Product");
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var entity = _context.Products.FirstOrDefault(i => i.Id == id);
        if (entity == null)
        {
            return View("Index");
        }
        string message;

        if (entity.Stock > 0)
        {
            message = $"{entity.ProductName} ürününün stokta {entity.Stock} adet var. Silmek istediğinizden emin misiniz?";
            ViewBag.ShowConfirmButton = true;
        }
        else if (entity.IsActive || entity.IsHome)
        {
            message = $"{entity.ProductName} ürünü sistemde aktif veya ana sayfada. Silinmedi, pasif hale getirildi.";
            entity.IsActive = false;
            entity.IsHome = false;
            _context.SaveChanges();

            TempData["Message"] = message;
            return RedirectToAction("Index");
        }
        else
        {
            message = $"{entity.ProductName} ürününü silmek istediğinizden emin misiniz?";
            ViewBag.ShowConfirmButton = true;
        }

        ViewBag.DeleteMessage = message;
        ViewBag.ProductId = entity.Id;

        return View(entity);
    }
    [HttpGet]
    public IActionResult DeleteConfirmPost(int? id)
    {
        if (id == null) return RedirectToAction("Index");

        var entity = _context.Products.FirstOrDefault(p => p.Id == id);
        if (entity == null) return RedirectToAction("Index");

        _context.Products.Remove(entity);
        _context.SaveChanges();

        TempData["Message"] = $"{entity.ProductName} silindi.";
        return RedirectToAction("Index");
    }

    [HttpPost("Product/DeleteConfirmPost")]
    public IActionResult DeleteConfirm(int? id)

    {
        var entity = _context.Products.FirstOrDefault(p => p.Id == id);
        if (entity == null)
        {
            return RedirectToAction("Index");
        }

        _context.Products.Remove(entity);
        _context.SaveChanges();

        TempData["Message"] = $"'{entity.ProductName}' başarıyla silindi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _context.Products.FirstOrDefault(p => p.Id == id);
        if (entity == null)
        {
            TempData["Message"] = "Ürün bulunamadı.";
            return RedirectToAction("Index");
        }

        if (entity.Stock > 0)
        {
            if (entity.IsActive || entity.IsHome)
            {
                entity.IsActive = false;
                entity.IsHome = false;
                _context.SaveChanges();

                TempData["Message"] = $"{entity.ProductName} ürünü stokta mevcut ve sistemde aktif/anasayfada. Silinemedi, pasif hale getirildi.";
                return RedirectToAction("Index");
            }
            else
            {
                _context.Products.Remove(entity);
                _context.SaveChanges();

                TempData["Message"] = $"{entity.ProductName} ürünü başarıyla silindi.";
                return RedirectToAction("Index");
            }
        }
        else
        {
            _context.Products.Remove(entity);
            _context.SaveChanges();

            TempData["Message"] = $"{entity.ProductName} ürünü başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<ActionResult> NonActiveProduct(int? category)
    {
        var products = await _context.Products
        .Include(p => p.ProductCategories)
        .ThenInclude(pc => pc.Category)
        .Where(p => !p.IsActive)
        .Select(p => new ProductGetModel
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Price = p.Price ?? 0,
            IsActive = p.IsActive,
            IsHome = p.IsHome,
            CategoryName = p.ProductCategories.FirstOrDefault().Category.CategoryName ?? "Kategori Yok",
            Stock = p.Stock,
            Image = p.Image
        })
        .ToListAsync();

        return View(products);
    }

}