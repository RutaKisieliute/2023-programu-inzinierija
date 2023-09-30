using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;

namespace AALKisMVCUI;

public class NotePageController : Controller
{
    private readonly ILogger<NotePageController> _logger;

    private readonly HttpClient _client;

    public NotePageController(ILogger<NotePageController> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
        return;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
