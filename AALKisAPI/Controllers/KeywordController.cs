using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared.Records;
using Newtonsoft.Json;

namespace AALKisAPI.Controllers;

/// Controllers tagged with ExcludeFromCodeCoverage should not, under any circumstance,
/// have important code, rather act as a bridge between requests and other parts of the code.
/// However, if there's a need to have important code (e.g. validate something before sending it out),
/// remove the tag from the class and apply it only to Http* methods,
/// with the important code being in functions called by Http* methods.
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[ApiController]
[Route("[controller]")]
public class KeywordController : ControllerBase
{
    private readonly ILogger<KeywordController> _logger;
    private readonly INotesRepository _notesRepository;
    private readonly IKeywordsRepository _keywordsRepository;

    public KeywordController(ILogger<KeywordController> logger,
            IKeywordsRepository keywordsRepository,
            INotesRepository notesRepository)
    {
        _logger = logger;
        _keywordsRepository = keywordsRepository;
        _notesRepository = notesRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Keyword>?> GetAllKeywords([FromBody] int userId)
    {
        try
        {
            return _keywordsRepository.GetAllKeywords(userId);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }
    
    [HttpGet("folder/{folderId}")]
    public async Task<IEnumerable<Keyword>?> GetFolderKeywords(int folderId)
    {
        try
        {
            return _keywordsRepository.GetAllKeywordsByFolder(folderId);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("note/{noteId}")]
    public async Task<IEnumerable<Keyword>?> GetNoteKeywords(int noteId)
    {
        try
        {
            return _keywordsRepository.GetAllKeywordsByNote(noteId);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("{noteId}/{name}")]
    public async Task<Keyword?> Get(int noteId, string name)
    {
        try
        {
            return _keywordsRepository.GetKeyword(name, noteId);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpHead("{name}")]
    public async Task<IActionResult> Exists(string name)
    {
        if(!_keywordsRepository.CheckIfKeywordExists(name))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("{noteId}/{name}")]
    public async Task<IActionResult> Create(int noteId, string name)
    {
        try
        {
            _keywordsRepository.CreateKeyword(name, noteId);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create Keyword {name} in note {noteId}: "
                    + exception.ToString());
            return BadRequest();
        }
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpDelete("{noteId}/{name}")]
    public async Task<IActionResult> Delete(int noteId, string name)
    {
        try
        {
            _keywordsRepository.DeleteKeyword(name, noteId);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to delete Keyword {name}: "
                    + exception.ToString());
            return BadRequest();
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPatch("{noteId}")]
    public async Task<IActionResult> Update(int noteId)
    {
        try
        {
            using var bodyStream = new StreamReader(Request.Body);
            string body = await bodyStream.ReadToEndAsync();

            List<Keyword> keywords = JsonConvert.DeserializeObject<List<Keyword>>(body)
                    ?? throw new Exception($"Could not deserialize {body} into list of keywords");

            _keywordsRepository.UpdateKeywordsForNote(keywords.Select(x => x.Name), noteId);

        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to update note's {noteId} keywords: "
                    + exception.ToString());
            return BadRequest();
        }

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}

