using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using System.Text.Json.Serialization;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;

namespace AALKisMVCUI.Controllers;

public class CategoriesController : Controller
{
    private readonly ILogger<HomeController> _logger;

    readonly Uri baseAddress = new Uri("https://localhost:7014");
    private readonly HttpClient _client;

    public CategoriesController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.BaseAddress = baseAddress;
    }

    public IActionResult Index()
    {
        List<string> cats = new List<string>(0);
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "CategoryList").Result;
        if(response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            cats = JsonSerializer.Deserialize<List<string>>(data);
        }
        var categories = new CategoriesModel
        {
            Categs = cats,
        };
        return View(categories);
    }

    public IActionResult C(string id)
    {
        List<string>? categs = new List<string>(0);
        List<string>? NoteList;
        string [] PostSplit;
        List<(string, string)> NoteList2 = new List<(string, string)>(0);
        HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "CategoryList").Result;
        if(response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            categs = JsonSerializer.Deserialize<List<string>>(data);
        }
        if(categs != null && categs.Contains(id))
        {
            response = _client.GetAsync(_client.BaseAddress + "NoteList?category=" + id).Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
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
        }
        return Error();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}