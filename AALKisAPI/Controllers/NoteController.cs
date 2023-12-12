using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;
using AALKisShared.Records;
using Newtonsoft.Json;
using System.Collections;

namespace AALKisAPI.Controllers;

/// Controllers tagged with ExcludeFromCodeCoverage should not, under any circumstance,
/// have important code, rather act as a bridge between requests and other parts of the code.
/// However, if there's a need to have important code (e.g. validate something before sending it out),
/// remove the tag from the class and apply it only to Http* methods,
/// with the important code being in functions called by Http* methods.
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly ILogger<NoteController> _logger;

    private readonly INotesRepository _notesRepository;

    public NoteController(ILogger<NoteController> logger,
            INotesRepository notesRepository)
    {
        _logger = logger;
        _notesRepository = notesRepository;
        _notesRepository.NoteCreated += NoteCreationLog;
    }

    [HttpGet("/Note")]
    public async Task<IEnumerable<Note>> GetAll()
    {
        try
        {
            return _notesRepository.GetAllNotes();
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }
    
    [HttpGet("{id}")]
    public async Task<Note?> Get(int id)
    {
        try
        {
            return _notesRepository.GetNote(id, previewOnly: false);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpHead("{id}")]
    public async Task<IActionResult> Exists(int id)
    {
        if(!_notesRepository.CheckIfNoteExists(id))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("[action]/{folderId}/{noteTitle}")]
    public async Task<int?> Create(int folderId, string noteTitle)
    {
        int? id = null;
        try
        {
            id = _notesRepository.CreateNote(folderId, noteTitle);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create note {noteTitle} in folder {folderId}: "
                    + exception.ToString());
        }
        return id;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _notesRepository.DeleteNote(id);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to delete note {id}: "
                    + exception.ToString());
            return BadRequest();
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            Note record = _notesRepository.GetNote(id, false);

            string jsonString = await new StreamReader(Request.Body).ReadToEndAsync();
            Note fieldsToUpdate = JsonConvert.DeserializeObject<Note>(jsonString);

            record.Update(
                    flags: record.Flags ^ fieldsToUpdate.Flags,
                    title: fieldsToUpdate.Title,
                    content: fieldsToUpdate.Content,
                    tags: fieldsToUpdate.Tags);

            record.EditDate = DateTime.Now;
            _notesRepository.UpdateNote(record);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Could not update note {id}: "
                    + exception.ToString());
            return BadRequest();
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("{folderId}/{id}")]
    public async Task<IActionResult> ChangeNoteFolder(int folderId, int id)
    {
        try
        {
            var record = _notesRepository.GetNote(id, false);
            _notesRepository.UpdateNote(record, folderId);
        }
        catch (Exception exception)
        {
            _logger.LogError($"Could not move note {id} from into {folderId}: "
                    + exception.ToString());
            return BadRequest();
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [NonAction]
    public void NoteCreationLog(object sender, Note note)
    {
        _logger.LogDebug("Successfully created new note: " + note.ToString());
    }

    [HttpGet("[action]/{query}")]
    public async Task<IEnumerable<Note>> Search(string query)
    {
        try
        {
            return _notesRepository.SearchNotes(query);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return new List<Note>();
    } 
}
