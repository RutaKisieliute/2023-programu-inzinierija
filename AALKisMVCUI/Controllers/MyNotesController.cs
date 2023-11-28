using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using AALKisShared.Enums;
using System.Text;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class MyNotesController : Controller
{
    private readonly ILogger<MyNotesController> _logger;
    private readonly APIClient _client;

    public MyNotesController(ILogger<MyNotesController> logger, APIClient client)
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
            folder.Records.Sort();
            folder.Records = folder.Records
                .Where(record => (record.Flags & NoteFlags.Archived) == 0)
                .ToList();
        }
        return View(folders);
    }

    [HttpPost("[action]/{folderId}")]
    public async Task<IActionResult> CreateEmptyNote(int folderId)
    {
        try 
        {
            string targetUri = $"/Note/Create/{folderId}/Untitled";
            var id = await _client
                    .Fetch<int?>(targetUri, HttpMethod.Post)
                    ?? throw new JsonException($"Got empty response from {targetUri}");

            return Json(new { redirectToUrl = "Editor/" + id});


        }
        catch (Exception ex) {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to create empty note\n" + ex.ToString());
            return BadRequest();
        }

    }

    [HttpPost("[action]/{folderName}")]
    public async Task<IActionResult> CreateEmptyFolder(string folderName)
    {
        try
        {
            string targetUri = "/Folder/" + folderName;
            var response = await _client
                    .Fetch(targetUri, HttpMethod.Post)
                    ?? throw new JsonException($"Got empty response from {targetUri}");
            return Ok();
        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to create empty folder\n" + ex.ToString());
            return BadRequest();
        }
    }

    [HttpPost("[action]/{noteId}")]
    public async Task<IActionResult> ArchiveNote(int noteId)
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

    [HttpPost("[action]/{folderId}/{noteId}")]
    public async Task<IActionResult> MoveNoteToFolder(int folderId, int noteId)
    {
        try
        {
            string targetUri = "/Note/" + folderId + "/" + noteId;
            var response = await _client.Fetch(targetUri, HttpMethod.Post)
                ?? throw new JsonException($"Got empty response from {targetUri}");

            return Ok();

        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to move note to other folder\n" + ex.ToString());
            return BadRequest();
        }

    }

    [HttpPost("[action]/{folderId}/{newName}")]
    public async Task<IActionResult> RenameFolder(int folderId, string newName)
    {
        try
        {
            string targetUri = "/Folder/" + folderId + "/" + newName;
            var response = await _client.Fetch(targetUri, HttpMethod.Patch)
                ?? throw new JsonException($"Got empty response from {targetUri}");

            return Ok();

        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to rename folder\n" + ex.ToString());
            return BadRequest();
        }

    }

}
