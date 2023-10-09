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
            record.Name = note;
            record.Text = record.Text.Replace("\n", "<br>");

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
            var record = new NoteRecord();
            record.Text = body.GetProperty("text").GetString()
                ?? throw new BadHttpRequestException("Got an empty text property");
            record.Name = note;

            record.Text = record.Text.Replace("<br>", "\n");
            record.Text = System.Web.HttpUtility.HtmlEncode(record.Text)
                .Replace("&amp;", "&");

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
