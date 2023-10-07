using Microsoft.AspNetCore.Mvc;
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

    // Returns all of the keywords with their contents
    [HttpGet("[action]")]
    public List<KeywordRecord>? Get()
    {
        return null;
    }

    // Returns keywords with only Name set.
    [HttpHead("[action]")]
    public List<KeywordRecord>? Head()
    {
        return null;
    }

    [HttpGet("[action]/{keyword}")]
    public KeywordRecord? Get(string keyword)
    {
        return null;
    }

}

