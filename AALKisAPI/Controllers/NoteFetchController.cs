using AALKisAPI.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteFetchController : ControllerBase
{
    private readonly ILogger<NoteFetchController> _logger;

    public NoteFetchController(ILogger<NoteFetchController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> Get(string id)
    {
        DatabaseService database = new DatabaseService();
        List<string> note = database.GetNote(id);
        note[(int) DatabaseService.COLUMN.Name] ??= "Error - note not found";
        note[(int) DatabaseService.COLUMN.Author] ??= "";
        note[(int) DatabaseService.COLUMN.Content] ??= "The note you're trying to reach has not been found";
        return note;
    }
}
