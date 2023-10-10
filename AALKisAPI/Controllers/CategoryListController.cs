using AALKisAPI.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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
        DatabaseService database = new DatabaseService();
        List<string> tags = database.GetTags().Where(str => str != null).ToList<string>();
        return tags;
    }
}
