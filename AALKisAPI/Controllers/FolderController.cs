using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FolderController : ControllerBase
{
    private readonly ILogger<FolderController> _logger;

    private readonly IRecordsService _recordsService;

    public FolderController(ILogger<FolderController> logger,
            IRecordsService recordsService)
    {
        _logger = logger;
        _recordsService = recordsService;
    }

    [HttpGet]
    public IEnumerable<FolderRecord<NoteRecord>>? Get([FromQuery] bool getContents = true)
    {
        try
        {
            return _recordsService.GetAllFolders(previewOnly: !getContents);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to get all folder records: "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("{folderName}")]
    public FolderRecord<NoteRecord>? Get(string folderName, [FromQuery] bool getContents = true)
    {
        try
        {
            return _recordsService.GetFolder(folderName, previewOnly: !getContents);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to get folder record \"{folderName}\": "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }


    [HttpHead("{folderName}")]
    public IActionResult Exists(string folderName)
    {
        if(!_recordsService.CheckIfFolderExists(folderName))
        {
            return new StatusCodeResult(StatusCodes.Status410Gone);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpPost("{folderName}")]
    public IActionResult Create(string folderName)
    {
        try
        {
            _recordsService.CreateFolder(folderName);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create folder record \"{folderName}\": "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpDelete("{folderName}")]
    public IActionResult Delete(string folderName, [FromQuery] bool force = false)
    {
        try
        {
            _recordsService.DeleteFolder(folderName, recursive: force);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to delete folder record \"{folderName}\": "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
