using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using AALKisMVCUI.Utility;
using AALKisShared;
using Newtonsoft.Json;

namespace AALKisMVCUI;

public class NotePageController : Controller
{
    private readonly ILogger<NotePageController> _logger;

    private readonly APIClient _client;

    public NotePageController(ILogger<NotePageController> logger, APIClient client)
    {
        _logger = logger;
        _client = client;
        return;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async void NotePost()
    {
        // Get the request's body as a string
        string body = await (new StreamReader(Request.Body)).ReadToEndAsync();
        _logger.Log(LogLevel.Error, $"Got {body}!");

        // Deserialize it as a note record
        NoteRecord record = JsonConvert.DeserializeObject<NoteRecord>(body);
        if(String.IsNullOrEmpty(record.Id) || record.Contents == null)
        {
            _logger.Log(LogLevel.Warning, $"Deserialized {body} into {record}"
                    + " had a null required property; stopping propogation to API.");
            return;
        }
        _logger.Log(LogLevel.Error, $"Deserialized to {record}!");
        record.Date = DateTime.Now;

        // Reserialize it and post it to the API
        string recordStr = JsonConvert.SerializeObject(record);
        _logger.Log(LogLevel.Error, $"Reserialized to {recordStr}!");
        var msg = new HttpRequestMessage(HttpMethod.Post, "");
        msg.Content = new StringContent(recordStr);
        return;
    }

    //[HttpGet]
    //public async Task<string> NoteGet()
    //{
    //    return "Foo!";
    //}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
