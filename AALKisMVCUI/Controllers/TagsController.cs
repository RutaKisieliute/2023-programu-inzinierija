using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using System.Text.Json.Serialization;
using Microsoft.JSInterop.Implementation;
using System.Text.Json;
using AALKisMVCUI.Utility;
using AALKisShared.Records;

namespace AALKisMVCUI.Controllers;

public class TagsController : Controller
{
    private readonly ILogger<TagsController> _logger;
    private readonly APIClient _client;

    public TagsController(ILogger<TagsController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet("[Controller]")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var tags = await _client.Fetch<List<Tag>>("/Tag", HttpMethod.Get) ?? throw new Exception("No tags returned");
            return View(tags);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return null;
    }

    [HttpGet("[Controller]/{tag}")]
    public async Task<IActionResult> GetTag(string tag)
    {
        try
        {
            var notes = await _client.Fetch<List<Note>>($"/Tag/{tag}", HttpMethod.Get) ?? throw new Exception("tag not found");
            return View(notes);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return Redirect("Tags/Error");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}