using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisMVCUI.Models;
using AALKisShared;
using Newtonsoft.Json;
using System.Text.Json;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class EditorController : Controller
{
    private readonly ILogger<EditorController> _logger;
    private readonly APIClient _client;

    public EditorController(ILogger<EditorController> logger, APIClient client)
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
        try
        {
            bool exists = await _client.Fetch<bool>($"NoteCatalog/Exists/{category}/{note}",
                    HttpMethod.Get);

            if(!exists)
            {
                throw new Exception($"{note} in {category} does not exist, yet attempted to open it");
            }

            return View(await GetNoteRecord(category, note));
        }
        catch(HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
        }
        return BadRequest();
    }

    [HttpGet("[action]/{category}/{note}")]
    public async Task<NoteRecord?> GetNoteRecord(string category, string note)
    {
        try
        {
            var record = await _client.Fetch<NoteRecord>($"NoteCatalog/Get/{category}/{note}",
                    HttpMethod.Get);
            record.Title = note;

            return record;
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get {category}/{note} NoteRecord;\n"
                    + e.ToString());
        }
        return null;
    }

    [HttpPost("[action]/{category}/{note}")]
    public async Task<IActionResult> PostNoteRecord(string category, string note, [FromBody] JsonElement body)
    {
        try
        {
            var record = await GetNoteRecord(category, note)
                ?? throw new HttpRequestException("Failed to get the note record.");
            record.Content = body.GetProperty("text").GetString()
                ?? throw new BadHttpRequestException("Got an empty text property");

            record.Content = record.Content.Replace("<br>", "\n");
            record.Content = System.Web.HttpUtility.HtmlEncode(record.Content)
                .Replace("&amp;", "&")
                .Replace("\n", "<br>");

            await _client.Fetch($"NoteCatalog/Put/{category}/{note}",
                    HttpMethod.Put,
                    new StringContent(record.ToJsonString()));
        }
        catch(HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch(Exception e)
        {
            _logger.LogError($"Failed to post {category}/{note} NoteRecord;\n"
                    + e.ToString());

            return BadRequest();
        }

        return Ok();
    }
}
