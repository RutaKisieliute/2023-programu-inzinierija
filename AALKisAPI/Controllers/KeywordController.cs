using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;
using Newtonsoft.Json;
using AALKisAPI.Models;
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

    [HttpGet("{noteId}/{name}")]
    public Keyword? Get(int noteId, string name)
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
    public IActionResult Exists(string name)
    {
        if(!_keywordsRepository.CheckIfKeywordExists(name))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("{noteId}/{name}")]
    public void Create(int noteId, string name)
    {
        try
        {
            _keywordsRepository.CreateKeyword(name, noteId);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create Keyword {name} in folder {noteId}: "
                    + exception.ToString());
        }
    }

    [HttpDelete("{noteId}/{name}")]
    public IActionResult Delete(int noteId, string name)
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

