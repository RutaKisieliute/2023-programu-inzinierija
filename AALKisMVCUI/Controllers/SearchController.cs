using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using AALKisShared.Enums;
using System.Text;
using AALKisMVCUI.Models;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class SearchController : Controller
{
    private readonly ILogger<MyNotesController> _logger;
    private readonly APIClient _client;
    private readonly IHttpContextAccessor _contextAccessor;

    public SearchController(ILogger<MyNotesController> logger, APIClient client, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _client = client;
        _contextAccessor = contextAccessor;
    }
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string q)
    {

        string targetUri = $"/Note/Search/{q}";
        int user_id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
        string json = JsonConvert.SerializeObject(user_id);
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        var notes = await _client
            .Fetch<List<Note>>(targetUri, HttpMethod.Get, content)
            ?? throw new JsonException($"Got empty response from {targetUri}");

        for (int i = 0; i < notes.Count; i++)
        {
            var note = notes[i];
            note.Title = InsertStringsBetweenAllSubstrings(note.Title, q, "<span class=\"highlight\">", "</span>");
            note.Content = InsertStringsBetweenAllSubstrings(note.Content, q, "<span class=\"highlight\">", "</span>");
            notes[i] = note;
        }

        var searchModel = new SearchViewModel { Notes = notes, SearchQuery = q};

        return View(searchModel);
    }

    public static string InsertStringsBetweenAllSubstrings(
        string original,
        string substring,
        string firstString,
        string secondString
    )
    {
        StringBuilder result = new StringBuilder(original);

        int index = 0;
        while ((index = result.ToString().IndexOf(substring, index, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            result.Insert(index, firstString);
            result.Insert(index + substring.Length + firstString.Length, secondString);
            index = index + substring.Length + firstString.Length + secondString.Length;
        }
        return result.ToString();
    }
}
