using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
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
            .Fetch<List<FolderRecord<NoteRecord>>>(targetUri, HttpMethod.Get)
            ?? throw new JsonException($"Got empty response from {targetUri}");
        folders.Sort();
        // Order by access date descending.
        foreach (var folder in folders)
        {
            folder.Records.Sort(); // = folder.Records.OrderByDescending(record => record.EditDate).ToList();
            folder.Records = folder.Records.FindAll(record => (record.Flags & NoteRecord.NoteFlags.Archived) != 0);
        }
        return View(folders);
    }
    
    [HttpPost("[action]/{folderName}/{noteName}")]
    public async Task<IActionResult> ArchiveNote(string folderName, string noteName)
    {
        try
        {
            string targetUri = "/Note/" + noteName + "/" + folderName;

            NoteRecord fieldsToUpdate = new NoteRecord();
            fieldsToUpdate.Flags = NoteRecord.NoteFlags.Archived; // Not sets, but switches.

            string jsonString = JsonConvert.SerializeObject(fieldsToUpdate);


            // Update Note Archived Flag
            await _client.Fetch($"Note/{folderName}/{noteName}",
                    HttpMethod.Put,
                    new StringContent(jsonString, Encoding.UTF8, "application/json"));

            return Ok();

        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to create EmptyNote\n" + ex.ToString());
            return BadRequest();
        }

    }

}
