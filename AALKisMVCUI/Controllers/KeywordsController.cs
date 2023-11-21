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
            var record = await _client.Fetch<IEnumerable<Keyword>>("/Keyword",
                    HttpMethod.Get);

            return View(record);
        }
        catch(Exception e)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            _logger.LogError($"Failed to get keywords;\n"
                    + e.ToString());
        }
        return null;
    }

    public IActionResult Privacy()
    {
        var tmp = new PrivacyViewModel();
        tmp.Message = "Hello world!";
        return View(tmp);
    }
    public IActionResult Dictionary()
    {
        var tmp = new PrivacyViewModel();
        tmp.Message = "Dictionary";
        return View(tmp);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
