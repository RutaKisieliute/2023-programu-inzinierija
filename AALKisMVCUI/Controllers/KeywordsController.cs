using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using AALKisMVCUI.Utility;
using AALKisShared.Records;

namespace AALKisMVCUI.Controllers;

public class KeywordsController : Controller
{
    private readonly ILogger<KeywordsController> _logger;

    private readonly APIClient _client;

    public KeywordsController(ILogger<KeywordsController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var records = await _client.Fetch<IEnumerable<Keyword>>("/Keyword",
                    HttpMethod.Get);

            return View(records);
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get keywords;\n"
                    + e.ToString());
        }
        return BadRequest();
    }
}
