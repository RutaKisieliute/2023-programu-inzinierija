using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisShared;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;

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
        IActionResult result = Ok();
        try
        {
            string body = await new StreamReader(Request.Body).ReadToEndAsync();

            NoteRecord fieldsToUpdate = new NoteRecord();
            fieldsToUpdate.SetFromJsonString(body);

            if(fieldsToUpdate.Title != null)
            {
                if(!fieldsToUpdate.IsTitleValid())
                {
                    throw new BadHttpRequestException("Tried to set title to non-valid string.");
                }
                fieldsToUpdate.Title = System.Web.HttpUtility
                    .HtmlEncode(fieldsToUpdate.Title)
                    .Replace("&amp;", "&");
                result = Content(fieldsToUpdate.Title);
            }

            if(fieldsToUpdate.Content != null)
            {
                fieldsToUpdate.Content = System.Web.HttpUtility
                    .HtmlEncode(fieldsToUpdate.Content)
                    .Replace("&amp;", "&");
            }


            // Passing json instead of string, so that PUT (/Note/{folderName}/{noteName}) can update not only content.
            string jsonString = JsonConvert.SerializeObject(fieldsToUpdate);

            await _client.Fetch($"Note/{folderName}/{noteName}",
                    HttpMethod.Put,
                    new StringContent(jsonString, Encoding.UTF8, "application/json"));
            
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

        return result;
    }
}
