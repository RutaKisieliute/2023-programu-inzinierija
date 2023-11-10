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

    private readonly INoteRecordsService _recordsService;

    public NoteController(ILogger<NoteController> logger,
            INoteRecordsService recordsService)
    {
        _logger = logger;
        _recordsService = recordsService;
    }

    [HttpGet("{id}")]
    public NoteRecord? Get(int id)
    {
        try
        {
            return _recordsService.GetNote(id, previewOnly: false);
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
        if(!_recordsService.CheckIfNoteExists(id))
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
            id = _recordsService.CreateNote(folderId, noteTitle);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create note {noteTitle} in folder {folderId}: "
                    + exception.ToString());
            return null;
        }
        if (id == -1) id = null;
        return id;
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _recordsService.DeleteNote(id);
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
            NoteRecord record = _recordsService.GetNote(id, false);

            string jsonString = await new StreamReader(Request.Body).ReadToEndAsync();
            NoteRecord fieldsToUpdate = JsonConvert.DeserializeObject<NoteRecord>(jsonString);

            record.Update(
                    flags: record.Flags ^ fieldsToUpdate.Flags,
                    title: fieldsToUpdate.Title,
                    content: fieldsToUpdate.Content);

            record.EditDate = DateTime.Now;
            _recordsService.UpdateNote(record);
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
            var record = _recordsService.GetNote(id, false);
            _recordsService.UpdateNote(record, folderId);
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
