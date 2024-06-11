using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyBookShelf.Models;

namespace MyBookShelf.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["Username"] = HttpContext.Session.GetString("username");
        ViewData["IsAdmin"] = HttpContext.Session.GetString("isadmin");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
