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
        string targetUri = "/Note";

        var notes = await _client
            .Fetch<List<Note>>(targetUri, HttpMethod.Get)
            ?? throw new JsonException($"Got empty response from {targetUri}");
        notes.Sort();
        // Order by access date descending.
        notes = notes
            .Where(note => note.FlagCheck(NoteFlags.Archived))
            .ToList();
        notes.Sort((a, b) => -1 * (a.EditDate ?? DateTime.UtcNow).CompareTo(b.EditDate));
        return View(notes);
    }
    
    [HttpPost("[action]/{noteId}")]
    public async Task<IActionResult> UnarchiveNote(int noteId)
    {
        try
        {
            string targetUri = "/Note/" + noteId;

            Note fieldsToUpdate = new Note
            {
                Flags = NoteFlags.Archived // Not sets, but switches.
            };

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

    [HttpDelete("[action]/{noteId}")]
    public async Task<IActionResult> DeleteNote(int noteId)
    {
        try
        {
            string targetUri = "/Note/" + noteId;
            var response = await _client.Fetch(targetUri, HttpMethod.Delete)
                ?? throw new JsonException($"Got empty response from {targetUri}");
            return Ok();
        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to delete note\n" + ex.ToString());
            return BadRequest();
        }

    }

}
