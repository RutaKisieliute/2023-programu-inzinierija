using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class DatabaseService
{
    private static readonly string DBConnection = "server=sql11.freesqldatabase.com;user=sql11651620;database=sql11651620;port=3306;password=HmgC9rDhfQ";

    public enum COLUMN
    {
        Name = 0,
        Author,
        Content
    }

    public MySqlConnection Con { get; }
    
    public DatabaseService()
    {
        Con = new MySqlConnection(DBConnection);
    }

    public List<string?> GetTags()
    {
        List<string?> list = new List<string?>();
        Con.Open();
        MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT tag FROM tags", Con);
        MySqlDataReader reader;
        try
        {
            reader = cmd.ExecuteReader();
        }
        catch(Exception)
        {
            return list;
        }
        while(reader.Read())
        {
            list.Add(reader["tag"].ToString());
        }
        reader.Close();
        Con.Close();
        return list;
    }

    public List<List<string?>> GetNotesByTag(string tag)
    {
        List<List<string?>> list = new List<List<string?>>();
        string query = "SELECT name, note_id FROM tags JOIN notes ON note_id = id WHERE public = true AND tag = '" + tag + "'";
        Con.Open();
        MySqlCommand cmd = new MySqlCommand(query, Con);
        MySqlDataReader reader;
        try
        {
            reader = cmd.ExecuteReader();
        }
        catch(Exception)
        {
            return list;
        }
        while(reader.Read())
        {
            list.Add(new List<string?>{reader["name"].ToString(), reader["note_id"].ToString()});
        }
        reader.Close();
        return list;
    }

    public List<string?> GetNote(string id)
    {
        List<string?> list = new List<string?>();
        string query = "SELECT name, author_username, note FROM notes WHERE id = " + id;
        Con.Open();
        MySqlCommand cmd = new MySqlCommand(query, Con);
        MySqlDataReader reader;
        try
        {
            reader = cmd.ExecuteReader();
        }
        catch(Exception)
        {
            return list;
        }
        reader.Read();
        list.Add(reader["name"].ToString());
        list.Add(reader["author_username"].ToString());
        list.Add(reader["note"].ToString());
        reader.Close();
        return list;
    }
}