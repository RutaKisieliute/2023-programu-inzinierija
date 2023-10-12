using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly ILogger<NoteController> _logger;

    private readonly IRecordsService _recordsService;

    public NoteController(ILogger<NoteController> logger,
            IRecordsService recordsService)
    {
        _logger = logger;
        _recordsService = recordsService;
    }

    [HttpGet("{folderName}/{noteTitle}")]
    public NoteRecord? Get(string folderName, string noteTitle)
    {
        try
        {
            return _recordsService.GetNote(folderName, noteTitle, previewOnly: false);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpHead("{folderName}/{noteTitle}")]
    public IActionResult Exists(string folderName, string noteTitle)
    {
        if(!_recordsService.CheckIfNoteExists(folderName, noteTitle))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
}

    [HttpPost("{folderName}/{noteTitle}")]
    public IActionResult Create(string folderName, string noteTitle)
    {
        try
        {
            _recordsService.CreateNote(folderName, noteTitle);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create note {noteTitle} in folder {folderName}: "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpDelete("{folderName}/{noteTitle}")]
    public IActionResult Delete(string folderName, string noteTitle)
    {
        try
        {
            _recordsService.DeleteNote(folderName, noteTitle);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to delete note {noteTitle} in {folderName}: "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPut("{folderName}/{noteTitle}")]
    public async Task<IActionResult> Put(string folderName, string noteTitle)
    {
        try
        {
            var record = _recordsService.GetNote(folderName, noteTitle, false);

            string body = await new StreamReader(Request.Body).ReadToEndAsync();
            record.Content = body;

            _recordsService.UpdateNote(folderName, record);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Could not update note {noteTitle} in {folderName}: "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
