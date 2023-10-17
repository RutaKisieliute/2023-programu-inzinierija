using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
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

    [HttpPost("[action]/{folderName}")]
    public async Task<IActionResult> CreateEmptyNote(string folderName)
    {
        try 
        {
            string targetUri;
            string noteName;

            // If "Untitled" taken, get "Untitled 1", if taken - "Untitled 2" ...
            for (int i = 0; ; i++) // breaks when status code = 410 - file doesn't exist.
            {
                noteName = "Untitled" + (i == 0 ? "" : " " + i.ToString());
                targetUri = "/Note/" + folderName + "/" + noteName;
                // Additional try/catch because APIClient throws exception when status code is not 2XX
                // In this case 410 is expected (file with such name doesn't exist).
                try
                {
                    var responseHead = await _client
                    .Fetch(targetUri, HttpMethod.Head)
                    ?? throw new JsonException($"Got empty response from {targetUri}");
                }
                catch(BadHttpRequestException e)
                {
                    if (e.StatusCode == 410)
                        break;
                    else
                        throw e;
                }
            }

            // Create Note
            var resposePost = await _client
                    .Fetch(targetUri, HttpMethod.Post)
                    ?? throw new JsonException($"Got empty response from {targetUri}");

            return Json(new { redirectToUrl = "/Editor/" + folderName + "/" + noteName});


        }
        catch (Exception ex) {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to create EmptyNote\n" + ex.ToString());
            return BadRequest();
        }

    }

    [HttpPost("[action]/{folderName}")]
    public async Task<IActionResult> CreateEmptyFolder(string folderName)
    {
        try
        {
            string targetUri = "/Folder/" + folderName;

            // Create Folder
            var response = await _client
                    .Fetch(targetUri, HttpMethod.Post)
                    ?? throw new JsonException($"Got empty response from {targetUri}");

            return Ok();


        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError($"Failed to create EmptyNote\n" + ex.ToString());
            return BadRequest();
        }

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
