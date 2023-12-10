using AALKisShared.Records;
using AALKisAPI.Services;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;

    private readonly ITagsRepository _tagsRepository;

    private readonly INotesRepository _notesRepository;

    public TagController(ILogger<TagController> logger, ITagsRepository tagsRepository, INotesRepository notesRepository)
    {
        _logger = logger;
        _tagsRepository = tagsRepository;
        _notesRepository = notesRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Tag>?> Get()
    {
        try
        {
            return _tagsRepository.GetAllTags();
        }
        catch(Exception exception)
        {
            _logger.LogError("Failed to get all tags: "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;        
    }

    [HttpGet("{name}")]
    public async Task<IEnumerable<Note>?> Get(string name)
    {
        try
        {
            return _notesRepository.GetAllNotes().Where(n => n.Tags?.Contains(name) ?? false && n.FlagCheck(AALKisShared.Enums.NoteFlags.Public));
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to get the tag {name}: "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;        
    }

    [HttpPut("{name}/{noteId}")]
    public async Task<IActionResult> Add(string name, int noteId)
    {
        try
        {
           _tagsRepository.AddTag(name, noteId); 
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to add the tag {name} to note {noteId}: "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;        
    }

    [HttpDelete("{name}/{noteId}")]
    public async Task<IActionResult> Delete(string name, int noteId)
    {
        try
        {
           _tagsRepository.DeleteTag(name, noteId); 
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed delete the tag {name} from note {noteId}: "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;        
    }
}
