using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;

public class ProductController : Controller
{
    //Dependecy Inject => DI
    private readonly DataContext _context;
    public ProductController(DataContext context)
    {
        _context = context;
    }
    public ActionResult Index(int? category)
    {
        var query = _context.Products.AsQueryable();
        if (category != null)
        {
            query = query.Where(i => i.CategoryId == category);
        }

        var products = query.Select(i => new ProductGetModel
        {
            Id = i.Id,
            ProductName = i.ProductName,
            Price = i.Price ?? 0,
            IsActive = i.IsActive,
            Image = i.Image,
            IsHome = i.IsHome,
            Stock = i.Stock,
            CategoryName = i.Category.CategoryName ?? "Kategori yok"
        }).ToList();
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName", category);

        return View(products);
    }
    public ActionResult List(string url, string q)
    {
        var query = _context.Products.Where(i => i.IsActive);

        if (!string.IsNullOrEmpty(url))
        {
            query = query.Where(i => i.Category.Url == url);
        }

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(i => i.ProductName.ToLower().Contains(q.ToLower()));

            ViewData["q"] = q;
        }

        //var products = _context.Products.Where(i => i.IsActive && i.Category.Url == url).ToList();
        return View(query.ToList());
    }
    public ActionResult Details(int id)
    {
        // var product = _context.Products.FirstOrDefault(p => p.Id == id);
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index", "Home");
        }
        ViewData["BenzerUrunler"] = _context.Products
            .Where(i => i.IsActive && i.CategoryId == product.CategoryId && i.Id != id)
            .Take(4)
            .ToList();
        return View(product);
    }
    [HttpGet("product/create")]
    public ActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
        return View();
    }

    [HttpPost("product/create")]
    public async Task<ActionResult> Create(ProductCreateModel model)
    {
        var fileName = Path.GetRandomFileName() + ".jpg";
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
        if (ModelState.IsValid)
        {
            if (model.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Image!.CopyToAsync(stream);
                }
            }
            else
            {
                fileName = "pho.png";
            }

            if (model.Stock == 0)
            {
                model.IsActive = false;
                model.IsHome = false;
            }

            var entity = new Product()
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                IsActive = model.IsActive,
                IsHome = model.IsHome,
                CategoryId = (int)model.CategoryId!,
                Image = fileName,
                Stock = (int)model.Stock!
            };

            _context.Products.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        return View(model);
    }
    [HttpGet]
    public ActionResult Edit(int id)
    {
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");

        var entity = _context.Products
        .Select(i => new ProductEditModel
        {
            Id = i.Id,
            ProductName = i.ProductName,
            Price = i.Price ?? 0,
            IsActive = i.IsActive,
            Image = i.Image,
            IsHome = i.IsHome,
            Stock = i.Stock,
            CategoryId = i.CategoryId,
            Description = i.Description
        })
        .FirstOrDefault(i => i.Id == id);
        if (entity == null)
        {
            return NotFound();
        }
        return View(entity);
    }
    [HttpPost]

    public async Task<ActionResult> Edit(int id, ProductEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");

        if (model.Stock == 0)
        {
            model.IsActive = false;
            model.IsHome = false;
        }

        if (ModelState.IsValid)
        {
            var entity = _context.Products.FirstOrDefault(i => i.Id == model.Id);

            if (entity != null)
            {
                if (model.ImageFile != null)
                {
                    var fileName = Path.GetRandomFileName() + ".jpg";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile!.CopyToAsync(stream);
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
                entity.CategoryId = model.CategoryId;
                entity.Description = model.Description;

                _context.SaveChanges();

                TempData["Message"] = $"`{entity.ProductName}` updated successfully!";
                return RedirectToAction("Index");
            }
        }
        else
        {
            var oldEntity = _context.Products.FirstOrDefault(p => p.Id == model.Id);
            if (oldEntity != null && string.IsNullOrEmpty(model.Image))
            {
                model.Image = oldEntity.Image;
            }

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "CategoryName");
            return View(model);
        }

        return View(model);
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
}