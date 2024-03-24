using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AALKisMVCUI.Models;
using AALKisMVCUI.Utility;
using AALKisShared.Records;
using Newtonsoft.Json;
using System.Text;

namespace AALKisMVCUI.Controllers;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[Route("[controller]")]
public class KeywordsController : Controller
{
    private readonly ILogger<KeywordsController> _logger;

    private readonly APIClient _client;
    private readonly IHttpContextAccessor _contextAccessor;

    public KeywordsController(ILogger<KeywordsController> logger, APIClient client, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _client = client;
        _contextAccessor = contextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            int user_id = _contextAccessor.HttpContext.Session.GetInt32("Id") ?? default;
            string json = JsonConvert.SerializeObject(user_id);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            IEnumerable<Keyword> keywords = await _client.Fetch<IEnumerable<Keyword>>("/Keyword",
                    HttpMethod.Get, content)
                ?? throw new Exception("Could not fetch keywords from API");

            IEnumerable<Folder<Note>> folders = await _client.Fetch<IEnumerable<Folder<Note>>>("/Folder?getContents=false",
                    HttpMethod.Get, content)
                ?? throw new Exception("Could not fetch folder names from API");

            var viewModels = keywords.Select(keyword => {
                    int folderId = (int)keyword.Origin?.OriginFolderId;
                    return new KeywordViewModel{
                        Name = (string)keyword.Name,
                        NoteId = (int)keyword.Origin?.Id,
                        NoteTitle = (string)keyword.Origin?.Title,
                        FolderId = folderId,
                        FolderName = (string)folders.FirstOrDefault(x => x.Id == folderId).Name};
                    });

            return View(viewModels);
        }
        catch(Exception e)
        {
            _logger.LogError($"Failed to get keywords;\n"
                    + e.ToString());
        }
        return BadRequest();
    }

    [HttpGet("Folder/{id}")]
    public async Task<IEnumerable<Keyword>?> GetFolderKeywords(int id)
    {
        try
        {
            var records = await _client.Fetch<IEnumerable<Keyword>>($"/Keyword/folder/{id}",
                    HttpMethod.Get);

            return records;
        }
        catch(Exception e)
        {
            _logger.LogError($"Failed to get keywords;\n"
                    + e.ToString());

            Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }
    }
}
