using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisMVCUI.Models;
using AALKisShared;
using Newtonsoft.Json;
using System.Text.Json;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class NoteEditorController : Controller
{
    private readonly ILogger<NoteEditorController> _logger;
    private readonly APIClient _client;

    public NoteEditorController(ILogger<NoteEditorController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public IActionResult Index()
    {
        return Redirect("Home/Error");
    }

    [HttpGet("{category}/{note}")]
    public async Task<IActionResult> Index(string category, string note)
    {
        string content = await _client.Fetch($"NoteCatalog/Exists/{category}/{note}", HttpMethod.Get)
            ?? throw new Exception($"Failed to check if {note} in {category} exists");

        if(!JsonConvert.DeserializeObject<bool>(content))
        {
            throw new Exception($"{note} in {category} does not exist, yet attempted to open it");
        }

        return View(await GetNoteRecord(category, note));
    }

    [HttpGet("[action]/{category}/{note}")]
    public async Task<NoteRecord> GetNoteRecord(string category, string note)
    {
        string content = await _client.Fetch($"NoteCatalog/Get/{category}/{note}", HttpMethod.Get)
            ?? throw new Exception($"Could not get note {note} in {category} that exists");

        var record = NoteRecord.FromJsonString(content);

        _logger.LogWarning($"{record}");

        return record;
    }

    [HttpPost("[action]/{category}/{note}")]
    public async void PostNoteRecord(string category, string note, [FromBody] JsonElement body)
    {
        NoteRecord record = NoteRecord.FromJsonString(body.GetRawText(), note);

        record.Text = record.Text.Replace("<br>", "\n");
        record.Text = System.Web.HttpUtility.HtmlEncode(record.Text)
            .Replace("&amp;", "&").Replace("\n", "<br>");

        await _client.Fetch($"NoteCatalog/Put/{category}/{note}", HttpMethod.Put, record.ToJsonString());
        return;
    }
}
