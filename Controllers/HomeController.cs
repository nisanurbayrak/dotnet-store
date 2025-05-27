using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;
    public HomeController(DataContext context)
    {
        _context = context;
    }
    [HttpGet]
    public ActionResult Index()
    {
        var products = _context.Products.Where(p => p.IsActive && p.IsHome).ToList();

        var categories = _context.Categories.Where(c => c.IsActive).ToList();
        var sliders = _context.Sliders.Include(s => s.Category).Where(s => s.Active).ToList();

        ViewBag.Categories = categories;
        ViewBag.Sliders = sliders;  // Ekle burayı

        return View(products);
    }

    [HttpGet]
    public IActionResult Deneme()
    {
        return View();
    }




}
