using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dotnet_store.Models;

namespace dotnet_store.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;
    public HomeController(DataContext context)
    {
        _context = context;
    }
    public ActionResult Index()
    {
        var products = _context.Products.Where(p => p.IsActive && p.IsHome).ToList();
        ViewData["Categories"] = _context.Categories.Where(c => c.IsActive == true).ToList();
        return View(products);
    }
}
