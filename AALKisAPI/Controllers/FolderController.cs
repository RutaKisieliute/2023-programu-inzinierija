using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using AALKisShared;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FolderController : ControllerBase
{
    private readonly ILogger<FolderController> _logger;

    private readonly IFoldersRepository _foldersRepository;

    public FolderController(ILogger<FolderController> logger,
            IFoldersRepository foldersRepository)
    {
        _logger = logger;
        _foldersRepository = foldersRepository;
    }

    [HttpGet]
    public IEnumerable<Folder<Note>>? Get([FromQuery] bool getContents = true)
    {
        try
        {
            return _foldersRepository.GetAllFolders(previewOnly: !getContents);
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
            return _foldersRepository.GetFolder(folderId, previewOnly: !getContents);
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
        if(!_foldersRepository.CheckIfFolderExists(folderId))
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
            _foldersRepository.CreateFolder(folderName);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to create folder record \"{folderName}\": "
                    + exception.ToString());
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPatch("{folderId}/{newFolderName}")]
    public IActionResult Rename(int folderId, string newFolderName)
    {
        try
        {
            _foldersRepository.RenameFolder(folderId, newFolderName);
        }
        catch(Exception exception)
        {
            _logger.LogError($"Failed to rename folder record {folderId} to \"{newFolderName}\": "
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
            _foldersRepository.DeleteFolder(folderId, force);
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
