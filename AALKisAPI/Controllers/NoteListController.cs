using AALKisAPI.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteListController : ControllerBase
{
    private readonly ILogger<NoteListController> _logger;

    public NoteListController(ILogger<NoteListController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> Get(string tag)
    {
        DatabaseService database = new DatabaseService();
        List<List<string?>> notes = database.GetNotesByTag(tag);
        List<string> converted = new List<string>();
        foreach(List<string?> list in notes)
        {
            converted.Add(list[0] + ";" + list[1]);
        }
        return converted;
    }
}
