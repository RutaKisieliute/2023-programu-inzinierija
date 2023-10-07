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
        List<string> list = new List<string>();
        using (MySqlConnection con = new MySqlConnection("server=sql11.freesqldatabase.com;user=sql11651620;database=sql11651620;port=3306;password=HmgC9rDhfQ"))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT tag FROM tags", con);
            MySqlDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return list;
            }
            while(reader.Read())
            {
                list.Add(reader["tag"].ToString());
            }
            reader.Close();
        }
        return list;
    }
}
