using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;

namespace AALKisMVCUI.Controllers;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        var tmp = new PrivacyViewModel();
        tmp.Message = "Hello world!";
        return View(tmp);
    }
    public IActionResult Dictionary()
    {
        var tmp = new PrivacyViewModel();
        tmp.Message = "Dictionary";
        return View(tmp);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
