using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisShared;
using AALKisShared.Exceptions;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(int id)
    {
        try
        {
            var response = await _client.Fetch($"Note/{id}",
                    HttpMethod.Head);

            if(!response.IsSuccessStatusCode)
            {
                throw new NoteException($"Note with {id} does not exist, yet attempted to open it");
            }

            return View(await GetNoteRecord(id));
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

    [HttpGet("[action]/{id}")]
    public async Task<NoteRecord?> GetNoteRecord(int id)
    {
        try
        {
            var record = await _client.Fetch<NoteRecord>($"Note/{id}",
                    HttpMethod.Get);

            return record;
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get {id} NoteRecord;\n"
                    + e.ToString());
        }
        return null;
    }

    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> PostNoteRecord(int id)
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
                    throw new NoteException(fieldsToUpdate,
                            $"Tried to set title to non-valid string \"{fieldsToUpdate.Title}\"");
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

            await _client.Fetch($"Note/{id}",
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
            _logger.LogError($"Failed to post {id} NoteRecord;\n"
                    + exception.ToString());

            return BadRequest();
        }

        return result;
    }
}
