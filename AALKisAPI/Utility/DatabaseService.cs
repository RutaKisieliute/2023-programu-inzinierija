using AALKisShared.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class DatabaseService
{
    private readonly string DBConnection;

    public enum COLUMN
    {
        Name = 0,
        User,
        Content
    }

    public MySqlConnection Con { get; }
    
    public DatabaseService()
    {
        DBConnection = File.ReadAllText("./Utility/databaselogin.txt");
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
        Con.Close();
        return list;
    }

    public List<string?> GetNote(string id)
    {
        List<string?> list = new List<string?>();
        string query = "SELECT name, author_id, note FROM notes WHERE id = " + id;
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
        list.Add(reader["author_id"].ToString());
        list.Add(reader["note"].ToString());
        reader.Close();
        Con.Close();
        return list;
    }
}
