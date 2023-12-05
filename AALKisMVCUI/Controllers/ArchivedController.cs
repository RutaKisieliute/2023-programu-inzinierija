using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using AALKisShared.Enums;
using System.Text;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class ArchivedController : Controller
{
    private readonly ILogger<ArchivedController> _logger;
    private readonly APIClient _client;

    public ArchivedController(ILogger<ArchivedController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        string targetUri = "/Folder";

        var folders = await _client
            .Fetch<List<Folder<Note>>>(targetUri, HttpMethod.Get)
            ?? throw new JsonException($"Got empty response from {targetUri}");
        folders.Sort();
        // Order by access date descending.
        foreach (var folder in folders)
        {
            folder.Records.Sort(); // = folder.Records.OrderByDescending(record => record.EditDate).ToList();
            folder.Records = folder.Records
                .Where(record => (record.Flags & NoteFlags.Archived) != 0)
                .ToList();
        }
        return View(folders);
    }
    
    [HttpPost("[action]/{noteId}")]
    public async Task<IActionResult> UnarchiveNote(int noteId)
    {
        try
        {
            string targetUri = "/Note/" + noteId;

            Note fieldsToUpdate = new Note();
            fieldsToUpdate.Flags = NoteFlags.Archived; // Not sets, but switches.

            string jsonString = JsonConvert.SerializeObject(fieldsToUpdate);
            var response = await _client.Fetch(targetUri, HttpMethod.Put, new StringContent(jsonString, Encoding.UTF8, "application/json"))
                ?? throw new JsonException($"Got empty response from {targetUri}");
            return Ok();
        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to archive note\n" + ex.ToString());
            return BadRequest();
        }

    }

}
