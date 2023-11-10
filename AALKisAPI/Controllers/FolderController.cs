using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FolderController : ControllerBase
{
    private readonly ILogger<FolderController> _logger;

    private readonly IFoldersService _recordsService;

    public FolderController(ILogger<FolderController> logger,
            IFoldersService recordsService)
    {
        _logger = logger;
        _recordsService = recordsService;
    }

    [HttpGet]
    public IEnumerable<Folder<Note>>? Get([FromQuery] bool getContents = true)
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

    [HttpGet("{folderId}")]
    public Folder<Note>? Get(int folderId, [FromQuery] bool getContents = true)
    {
        try
        {
            return _recordsService.GetFolder(folderId, previewOnly: !getContents);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to get folder record \"{folderId}\": "
                    + exception.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }


    [HttpHead("{folderId}")]
    public IActionResult Exists(int folderId)
    {
        if(!_recordsService.CheckIfFolderExists(folderId))
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

    [HttpPatch("{folderName}/{newFolderName}")]
    public IActionResult Rename(string folderName, string newFolderName)
    {
        try
        {
            _recordsService.RenameFolder(folderName, newFolderName);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to rename folder record \"{folderName}\" to \"{newFolderName}\": "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }


    [HttpDelete("{folderId}")]
    public IActionResult Delete(int folderId, [FromQuery] bool force = false)
    {
        try
        {
            _recordsService.DeleteFolder(folderId, force);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to delete folder record \"{folderId}\": "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
