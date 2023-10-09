using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using System.Text.Json.Serialization;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;
using AALKisMVCUI.Utility;

namespace AALKisMVCUI.Controllers;

public class CategoriesController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly APIClient _client;

    public CategoriesController(ILogger<HomeController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        List<string> categs = new List<string>(0);
        string? data = await _client.Fetch("/CategoryList", HttpMethod.Get);
        try
        {
            categs = JsonSerializer.Deserialize<List<string>>(data);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            categs = new List<string> {"Error"};
        }
        var categories = new CategoriesModel
        {
            Categs = categs,
        };
        return View(categories);
    }

    public async Task<IActionResult> C(string id)
    {
        List<string>? categs = new List<string>(0);
        List<string>? NoteList;
        string [] PostSplit;
        List<(string, string)> NoteList2 = new List<(string, string)>(0);
        string? data = await _client.Fetch("/CategoryList", HttpMethod.Get);
        try
        {
            categs = JsonSerializer.Deserialize<List<string>>(data);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            categs = null;
        }
        if(categs != null && categs.Contains(id))
        {
            data = await _client.Fetch($"/NoteList?tag={id}", HttpMethod.Get);
            try
            {
                NoteList = JsonSerializer.Deserialize<List<string>>(data);
                foreach(string str in NoteList)
                {
                    PostSplit = str.Split(';');
                    NoteList2.Add((PostSplit[0], PostSplit[1]));
                }
                var notes = new CategoryModel
                {
                    Notes = NoteList2,
                    Name = id
                };
                return View(notes);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return Redirect("Categories/Error");
            }
        }
        return Redirect("Categories/Error");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}