using Microsoft.AspNetCore.Mvc;
using AALKisShared;
using AALKisShared.Utility;

namespace AALKisAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class NoteCategoriesController : ControllerBase
{
    private readonly ILogger<NoteCategoriesController> _logger;

    public NoteCategoriesController(ILogger<NoteCategoriesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<NoteCategoryRecord> Get()
    {
        return (from dirPath in Directory.GetDirectories("./DataBase/Categories")
                select NoteCategoryRecord.FromDirectory(dirPath)).ToList();
    }
}
