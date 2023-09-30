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
        return new List<string>{"Biology", "Chemistry", "Physics", "IT", "Math"};
    }
}
