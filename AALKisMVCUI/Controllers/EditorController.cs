using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using AALKisShared.Exceptions;
using AALKisShared.Utility;
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

            return View(await GetNote(id));
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
    public async Task<Note?> GetNote(int id)
    {
        try
        {
            var record = await _client.Fetch<Note>($"Note/{id}",
                    HttpMethod.Get);

            return record;
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get {id} Note;\n"
                    + e.ToString());
        }
        return null;
    }

    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> PostNote(int id)
    {
        IActionResult result = Ok();
        try
        {
            string body = await new StreamReader(Request.Body).ReadToEndAsync();

            Note fieldsToUpdate = CreateValidatedNote(body, id);

            string jsonString = JsonConvert.SerializeObject(fieldsToUpdate);

            await _client.Fetch($"Note/{id}",
                    HttpMethod.Put,
                    new StringContent(jsonString, Encoding.UTF8, "application/json"));

            if(fieldsToUpdate.Title != null)
            {
                result = Content(fieldsToUpdate.Title);
            }
        }
        catch(HttpRequestException exception)
        {
            _logger.LogError(exception.ToString());
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to post {id} Note;\n"
                    + exception.ToString());

            return BadRequest();
        }

        return result;
    }

    public static Note CreateValidatedNote(string json, int? id = null)
    {
        Note validatedNote = new Note();
        validatedNote.SetFromJsonString(json);
        validatedNote.Id = id ?? validatedNote.Id;


        if(validatedNote.Id < 0)
        {
            throw new NoteException(validatedNote,
                    $"Invalid id while validating note");
        }

        if(validatedNote.Title != null)
        {
            if(!validatedNote.IsTitleValid())
            {
                throw new NoteException(validatedNote,
                        $"Invalid title \"{validatedNote.Title}\" while validating note");
            }
            validatedNote.Title = System.Web.HttpUtility
                .HtmlEncode(validatedNote.Title)
                .Replace("&amp;", "&");
        }

        if(validatedNote.Content != null)
        {
            if(!validatedNote.IsContentValid())
            {
                throw new NoteException(validatedNote,
                        $"Invalid content while validating note");
            }
            validatedNote.Content = System.Web.HttpUtility
                .HtmlEncode(validatedNote.Content)
                .Replace("&amp;", "&");
        }

        return validatedNote;
    }
}
