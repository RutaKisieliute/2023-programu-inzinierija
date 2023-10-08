using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using System.Text.Json.Serialization;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;
using AALKisMVCUI.Utility;

namespace AALKisMVCUI.Controllers;

public class NoteController : Controller
{
    private readonly ILogger<HomeController> _logger;

    readonly Uri baseAddress = new Uri("https://localhost:7014");
    //private readonly HttpClient _client;
    private readonly APIClient _client;

    public NoteController(ILogger<HomeController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public IActionResult Index()
    {
        return Redirect("/Note/Error");
    }

    public async Task<IActionResult> N(string id)
    {
        string? data = null;
        try
        {
            data = await _client.Fetch($"/NoteFetch?id={id}", HttpMethod.Get);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return View("error");
        }
        List<string> NoteData = JsonSerializer.Deserialize<List<string>>(data) ?? new List<string>{"", "", ""};
        NoteModel note = new NoteModel{
            Name = NoteData[0],
            Author = NoteData[1],
            Content = NoteData[2]
        };
        return View(note);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
