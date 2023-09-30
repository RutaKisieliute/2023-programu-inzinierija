using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using System.Text.Json.Serialization;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;

namespace AALKisMVCUI.Controllers;

public class NoteController : Controller
{
    private readonly ILogger<HomeController> _logger;

    readonly Uri baseAddress = new Uri("https://localhost:7014");
    private readonly HttpClient _client;

    public NoteController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.BaseAddress = baseAddress;
    }

    public IActionResult Index()
    {
        return Error();
    }

    public IActionResult N(string code)
    {
        //API call goes here
        return View(new NoteModel{text = "Notes are not implemented yet."});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
