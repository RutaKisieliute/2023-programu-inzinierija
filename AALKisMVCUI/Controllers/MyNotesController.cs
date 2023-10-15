using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AALKisMVCUI.Utility;
using AALKisShared;
using System.Net;
using Microsoft.AspNetCore.Http.Extensions;

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
        // Order by access date descending.
        foreach (var folder in folders)
        {
            folder.Records = folder.Records.OrderByDescending(record => record.EditDate).ToList();
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

}
