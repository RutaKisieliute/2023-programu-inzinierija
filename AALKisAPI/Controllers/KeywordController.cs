using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using Newtonsoft.Json;
using AALKisShared.Records;
using System.Collections.Generic;
using System.Linq;

namespace AALKisAPI.Controllers;

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
    public async Task<IEnumerable<Keyword>?> GetAllKeywords()
    {
        try
        {
            return _keywordsRepository.Filter(EFKeywordsRepository.nothing, 0);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("name/{name}")]
    public async Task<IEnumerable<Keyword>?> GetNameKeywords(string name)
    {
        try
        {
            return _keywordsRepository.Filter(EFKeywordsRepository.name, name);
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
            return _keywordsRepository.Filter(EFKeywordsRepository.folder, folderId);
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
            return _keywordsRepository.Filter(EFKeywordsRepository.note, noteId);
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
}

