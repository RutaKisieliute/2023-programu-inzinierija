using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared;

namespace AALKisMVCUI.Controllers;

public class NotesViewController : Controller
{
    private readonly ILogger<NotesViewController> _logger;
    private readonly APIClient _client;

    public NotesViewController(ILogger<NotesViewController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        string targetUri = "/NoteCategories";

        string contents = await _client.GetContents(targetUri);

        var categories = JsonConvert.DeserializeObject<List<NoteCategoryRecord>>(contents)
            ?? throw new JsonException($"Got empty response from {targetUri}");

        return View(categories);
    }
}
