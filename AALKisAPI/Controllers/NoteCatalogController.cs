using Microsoft.AspNetCore.Mvc;
using AALKisShared;
using AALKisShared.Utility;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteCatalogController : ControllerBase
{
    private readonly ILogger<NoteCatalogController> _logger;

    private readonly string baseDirectory = "./DataBase/Catalog";

    public NoteCatalogController(ILogger<NoteCatalogController> logger)
    {
        _logger = logger;
    }

    [HttpGet("[action]")]
    public IEnumerable<NoteCategoryRecord>? Get([FromQuery] bool getNoteContents = true)
    {
        try
        {
            return from dirPath in Directory.GetDirectories(baseDirectory)
                    select NoteCategoryRecord.FromDirectory(
                            readNoteContents: getNoteContents,
                            path: dirPath);
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("[action]/{category}")]
    public NoteCategoryRecord? Get(string category, [FromQuery] bool getNoteContents = true)
    {
        try
        {
            return NoteCategoryRecord.FromDirectory(
                    readNoteContents: getNoteContents,
                    path: $"{baseDirectory}/{category}");
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("[action]/{category}/{note}")]
    public NoteRecord? Get(string category, string note)
    {
        try
        {
            return NoteRecord.FromJsonFile($"{baseDirectory}/{category}/{note}.json");
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("[action]/{category}")]
    public bool Exists(string category)
    {
        return Directory.Exists($"{baseDirectory}/{category}");
    }

    [HttpGet("[action]/{category}/{note}")]
    public bool Exists(string category, string note)
    {
        return System.IO.File.Exists($"{baseDirectory}/{category}/{note}.json");
    }

    [HttpPost("[action]/{category}")]
    public void Create(string category)
    {
        Response.StatusCode = StatusCodes.Status201Created;

        if(Exists(category))
        {
            _logger.LogError($"Attempted to create an existing category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        Directory.CreateDirectory($"{baseDirectory}/{category}");

        return;
    }

    [HttpPost("[action]/{category}/{note}")]
    public void Create(string category, string note)
    {
        Response.StatusCode = StatusCodes.Status201Created;

        if(!Exists(category))
        {
            _logger.LogError($"Attempted to create a note {note} for missing category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        if(Exists(category, note))
        {
            _logger.LogError($"Attempted to create an existing note {note} for category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        (new NoteRecord {Name = note, Text = ""}).SaveToJsonFile($"{baseDirectory}/{category}");
        return;
    }

    [HttpDelete("[action]/{category}")]
    public void Delete(string category)
    {
        if(!Exists(category))
        {
            _logger.LogError($"Attempted to delete missing category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        Directory.Delete($"{baseDirectory}/{category}", true);

        return;
    }

    [HttpDelete("[action]/{category}/{note}")]
    public void Delete(string category, string note)
    {
        if(!Exists(category))
        {
            _logger.LogError($"Attempted to delete note {note} for missing category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        if(!Exists(category, note))
        {
            _logger.LogError($"Attempted to delete missing note {note} for category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        System.IO.File.Delete($"{baseDirectory}/{category}/{note}.json");

        return;
    }

    [HttpPut("[action]/{category}/{note}")]
    public async void Put(string category, string note)
    {
        if(!Exists(category))
        {
            _logger.LogError($"Attempted to put to missing category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        if(!Exists(category, note))
        {
            _logger.LogError($"Attempted to put to missing note {note} from category {category}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        string body = await new StreamReader(Request.Body).ReadToEndAsync();

        NoteRecord record = NoteRecord.FromJsonString(body, note);
        record.SaveToJsonFile($"{baseDirectory}/{category}");

        return;
    }
}
