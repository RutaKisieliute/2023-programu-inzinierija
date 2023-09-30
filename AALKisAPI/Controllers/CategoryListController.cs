using Microsoft.AspNetCore.Mvc;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryListController : ControllerBase
{
    private readonly ILogger<CategoryListController> _logger;

    public CategoryListController(ILogger<CategoryListController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        //right now it reads from a .txt file but eventually it should read from a database
        string input;
        List<string> list = new List<string>();
        StreamReader reader = new StreamReader("../DataBase/Categories.txt");
        try
        {
            do
            {
                input = reader.ReadLine();
                list.Add(input);
            }while(input != null);
        }
        catch(Exception e)
        {
            return null;
        }
        return list;
    }
}
