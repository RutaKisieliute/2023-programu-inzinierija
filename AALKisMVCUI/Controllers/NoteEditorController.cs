using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Utility;
using AALKisMVCUI.Models;

namespace AALKisMVCUI.Controllers;

[Route("[controller]")]
public class NoteEditorController : Controller
{
    private readonly ILogger<NoteEditorController> _logger;
    private readonly APIClient _client;

    public NoteEditorController(ILogger<NoteEditorController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
    }

    public IActionResult Index()
    {
        throw new BadHttpRequestException("", 404);
        return Error();
    }

    [HttpGet("{category}/{note}")]
    public IActionResult Index(string category, string note)
    {
        _logger.LogWarning($"{category} {note}");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
