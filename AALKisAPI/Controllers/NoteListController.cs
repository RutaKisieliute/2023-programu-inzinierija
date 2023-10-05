using Microsoft.AspNetCore.Mvc;

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
    public IEnumerable<string> Get(string category)
    {
        string adress = "DataBase/";
        string input;
        List<string> list = new List<string>();
        StreamReader reader = new StreamReader(adress + category + ".txt");
        try
        {
            input = reader.ReadLine();
            while(input != null)
            {
                list.Add(input);
                input = reader.ReadLine();
            }
        }
        catch(Exception e)
        {
            return null;
        }
        return list;
    }
}
