using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using AALKisShared.Exceptions;
using AALKisShared.Utility;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class EditorController : Controller
{
    private readonly ILogger<EditorController> _logger;
    private readonly APIClient _client;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public EditorController(ILogger<EditorController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public IActionResult Index()
    {
        return Redirect("Home/Error");
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> PostNote(int id)
    {
        IActionResult result = Ok();
        try
        {
            string body = await new StreamReader(Request.Body).ReadToEndAsync();

            Note fieldsToUpdate = CreateValidatedNote(body);
            fieldsToUpdate.Id = id;

            HashSet<Keyword> keywords = GetKeywordsFromString(fieldsToUpdate.Content);
            foreach(Keyword keyword in keywords)
            {
                _logger.LogDebug($"Found keyword {keyword}");
            }

            string noteJsonString = fieldsToUpdate.ToJsonString();
            string keywordJsonString = JsonConvert.SerializeObject(keywords.ToList());

            var noteResponse = await _client.Fetch($"Note/{id}",
                    HttpMethod.Put,
                    new StringContent(noteJsonString, Encoding.UTF8, "application/json"));

            noteResponse.EnsureSuccessStatusCode();

            var keywordResponse = await _client.Fetch($"Keyword/{id}",
                    HttpMethod.Patch,
                    new StringContent(keywordJsonString, Encoding.UTF8, "application/json"));

            keywordResponse.EnsureSuccessStatusCode();

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

    public static Note CreateValidatedNote(string json)
    {
        Note validatedNote = new Note();
        validatedNote.SetFromJsonString(json);

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

    public static HashSet<Keyword> GetKeywordsFromString(string? content)
    {
        HashSet<Keyword> result = new HashSet<Keyword>();
        if(content != null)
        {
            var regex = new Regex(@"\$([A-z]+)", RegexOptions.IgnoreCase);
            foreach(Match match in regex.Matches(content))
            {
                result.Add(new Keyword{Name = match.Groups[1].Value.ToLower()});
            }
        }
        return result;
    }
}
