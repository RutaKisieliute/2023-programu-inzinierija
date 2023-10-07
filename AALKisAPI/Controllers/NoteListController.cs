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
    public IEnumerable<string> Get(string category)
    {
        List<string> list = new List<string>();
        string query = "SELECT name, note_id FROM tags JOIN notes ON note_id = id WHERE tag = '" + category + "'";
        using (MySqlConnection con = new MySqlConnection("server=sql11.freesqldatabase.com;user=sql11651620;database=sql11651620;port=3306;password=HmgC9rDhfQ"))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch(Exception e)
            {
                _logger.LogError("error here");
                _logger.LogError(e.ToString());
                return list;
            }
            while(reader.Read())
            {
                list.Add((reader["name"].ToString() ?? "a") + ";" + (reader["note_id"].ToString() ?? "aaa"));
            }
            reader.Close();
        }
        /*string adress = "DataBase/";
        string input;
        
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
        }*/
        return list;
    }
}
