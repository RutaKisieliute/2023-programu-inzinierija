using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisShared;
using System.Text.Json;
using Newtonsoft.Json;

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

    [HttpGet("{folderName}/{noteName}")]
    public async Task<IActionResult> Index(string folderName, string noteName)
    {
        try
        {
            var response = await _client.Fetch($"Note/{folderName}/{noteName}",
                    HttpMethod.Head);

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"{noteName} in {folderName} does not exist, yet attempted to open it");
            }

            return View(await GetNoteRecord(folderName, noteName));
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

    [HttpGet("[action]/{folderName}/{noteName}")]
    public async Task<NoteRecord?> GetNoteRecord(string folderName, string noteName)
    {
        try
        {
            var record = await _client.Fetch<NoteRecord>($"Note/{folderName}/{noteName}",
                    HttpMethod.Get);
            record.Title = noteName;

            return record;
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get {folderName}/{noteName} NoteRecord;\n"
                    + e.ToString());
        }
        return null;
    }

    [HttpPost("[action]/{folderName}/{noteName}")]
    public async Task<IActionResult> PostNoteRecord(string folderName, string noteName)
    {
        try
        {
            string body = await new StreamReader(Request.Body).ReadToEndAsync();

            body = body.Replace("<br>", "\n");
            body = System.Web.HttpUtility.HtmlEncode(body)
                .Replace("&amp;", "&")
                .Replace("\n", "<br>");

            await _client.Fetch($"Note/{folderName}/{noteName}",
                    HttpMethod.Put,
                    new StringContent(body));
        }
        catch(HttpRequestException exception)
        {
            _logger.LogError(exception.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to post {folderName}/{noteName} NoteRecord;\n"
                    + exception.ToString());

            return BadRequest();
        }

        return Ok();
    }
}
