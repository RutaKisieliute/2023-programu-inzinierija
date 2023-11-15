using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;
using Newtonsoft.Json;

namespace AALKisAPI.Controllers;

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
    }

    [HttpGet("{id}")]
    public Note? Get(int id)
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
    public IActionResult Exists(int id)
    {
        if(!_notesRepository.CheckIfNoteExists(id))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("[action]/{folderId}/{noteTitle}")]
    public int? Create(int folderId, string noteTitle)
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
    public IActionResult Delete(int id)
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
                    content: fieldsToUpdate.Content);

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
    public IActionResult ChangeNoteFolder(int folderId, int id)
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
}
