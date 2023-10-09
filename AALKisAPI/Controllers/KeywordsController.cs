using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AALKisShared;
using AALKisShared.Utility;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class KeywordsController : ControllerBase
{
    private readonly ILogger<KeywordsController> _logger;

    private readonly string baseDirectory = "./DataBase/Keywords";

    public KeywordsController(ILogger<KeywordsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("[action]")]
    public IEnumerable<KeywordRecord> Get([FromQuery] bool getContents = true)
    {
        return from file in Directory.GetFiles(baseDirectory)
                select KeywordRecord.FromJsonFile(file, getContents);
    }

    [HttpGet("[action]/{keyword}")]
    public KeywordRecord? Get(string keyword)
    {
        try
        {
            KeywordRecord record = KeywordRecord.FromJsonFile($"{baseDirectory}/{keyword}");
            return record;
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return null;
    }

    [HttpGet("[action]/{keyword}")]
    public bool Exists(string keyword)
    {
        if(System.IO.File.Exists($"{baseDirectory}/{keyword}.json"))
        {
            return true;
        }
        return false;
    }

    [HttpPost("[action]/{keyword}")]
    public void Create(string keyword, [FromBody] JsonElement body)
    {
        Response.StatusCode = StatusCodes.Status201Created;
        try
        {
            if(Exists(keyword))
            {
                throw new BadHttpRequestException("Attempted to post an existing KeywordRecord");
            }

            KeywordRecord record = KeywordRecord.FromJsonString(body.GetRawText(), keyword);
            record.SaveToJsonFile(baseDirectory);
        }
        catch(Exception e)
        {
            _logger.LogError(e.ToString());
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return;
    }

    [HttpDelete("[action]/{keyword}")]
    public void Delete(string keyword)
    {
        if(!Exists(keyword))
        {
            _logger.LogError($"Attempted to delete missing keyword {keyword}");
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        System.IO.File.Delete($"{baseDirectory}/{keyword}.json");
        return;
    }

}

