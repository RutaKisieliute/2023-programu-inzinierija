using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using Newtonsoft.Json;
using System.Text;

namespace AALKisMVCUI.Controllers;

public class TagsController : Controller
{
    private readonly ILogger<TagsController> _logger;
    private readonly APIClient _client;
    private readonly IHttpContextAccessor _contextAccessor;

    public TagsController(ILogger<TagsController> logger, APIClient client, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _client = client;
        _contextAccessor = contextAccessor;
    }

    [HttpGet("[Controller]")]
    public async Task<IActionResult> Index()
    {
        try
        {
            int user_id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
            string json = JsonConvert.SerializeObject(user_id);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var tags = await _client.Fetch<List<Tag>>("/Tag", HttpMethod.Get, content) ?? throw new Exception("No tags returned");
            return View(tags);
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        return Redirect("Tags/Error");
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
