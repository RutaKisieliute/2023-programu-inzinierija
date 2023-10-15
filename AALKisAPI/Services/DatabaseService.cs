using System.Text;

using AALKisAPI.Services;

using AALKisShared;
using AALKisShared.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class DatabaseService : IRecordsService
{
    private readonly string DBConnection;

    public enum COLUMN
    {
        Name = 0,
        User,
        Content
    }

    //public MySqlConnection Con { get; }
    
    public DatabaseService()
    {
        DBConnection = File.ReadAllText("./Services/databaselogin.txt");
        //Con = new MySqlConnection(DBConnection);
    }

    /*public List<string?> GetTags()
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
    }*/

    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly)
    {
        List<FolderRecord<NoteRecord>> folders = new List<FolderRecord<NoteRecord>>();
        string query = "SELECT title FROM  folders";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                try
                {
                    folders.Add(GetFolder(reader["title"].ToString() ?? "", false));
                }
                catch(Exception){}
            }
            return folders;
        }
        catch(Exception e)
        {
            Console.WriteLine("ERROR ERROR HERE -->" + e.Message + "!!!!!");
            return folders;
        }

    }

    public FolderRecord<NoteRecord> GetFolder(string folderName, bool previewOnly)
    {
        if(!CheckIfFolderExists(folderName)) throw new Exception("The folder doesn't exist");
        string query1 = $"SELECT id FROM folders WHERE title = '{folderName}'";
        string query2 = $"SELECT notes.* FROM folders, notes WHERE folders.title = '{folderName}' AND notes.folder_id = folders.id";
        NoteRecord note;
        FolderRecord<NoteRecord> folder = new FolderRecord<NoteRecord>(){Name = folderName};
        MySqlDataReader reader;
        try
        {
            using (MySqlConnection connection1 = new MySqlConnection(DBConnection))
            using (MySqlCommand cmd1 = new MySqlCommand(query1, connection1))
            {
                connection1.Open();
                reader = cmd1.ExecuteReader();
                reader.Read();
                folder.Id = Convert.ToInt32(reader["id"]);
            }
            using (MySqlConnection connection2 = new MySqlConnection(DBConnection))
            using (MySqlCommand cmd2 = new MySqlCommand(query2, connection2))
            {
                connection2.Open();
                reader = cmd2.ExecuteReader();
                while(reader.Read())
                {
                    note = new NoteRecord();
                    note.Id = Convert.ToInt64(reader["id"]);
                    note.Title = reader["title"].ToString() ?? "";
                    note.Content = reader["content"].ToString();
                    if(Convert.ToBoolean(reader["public"])) note.Flags = note.Flags | NoteRecord.NoteFlags.Public;
                    folder.Records.Add(note);
                }
            }
            return folder;
        }
        catch(Exception)
        {
            //backup?
            return folder;
        }
    }

    public NoteRecord GetNote(string folderName, string noteTitle, bool previewOnly)
    {
        if(!CheckIfNoteExists(folderName, noteTitle)) throw new Exception("The note doesn't exist");
        string query = $"SELECT notes.* FROM folders, notes WHERE folders.title = '{folderName}' AND notes.title = '{noteTitle}' AND notes.folder_id = folders.id";
        NoteRecord note = new NoteRecord();
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            note.Id = Convert.ToInt64(reader["id"]);
            note.Title = reader["title"].ToString() ?? "";
            note.Content = reader["content"].ToString();
            if(Convert.ToBoolean(reader["public"])) note.Flags = note.Flags | NoteRecord.NoteFlags.Public;
            return note;
        }
        catch(Exception)
        {
            //backup?
            return new NoteRecord(){Content = null};
        }
    }

    public bool CheckIfFolderExists(string folderName)
    {
        string query = $"SELECT title FROM folders WHERE title = '{folderName}'";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read()) return true;
            return false;
        }
        catch(Exception)
        {
            //backup?
            return false;
        }
    }

    public bool CheckIfNoteExists(string folderName, string noteTitle)
    {
        string query = $"SELECT notes.title FROM folders, notes WHERE folders.title = '{folderName}' AND notes.title = '{noteTitle}' AND notes.folder_id = folders.id";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read()) return true;
            return false;
        }
        catch(Exception)
        {
            //backup?
            return false;
        }

    }

    public void CreateFolder(string folderName){}

    public void CreateNote(string folderName, string noteTitle){}

    public void DeleteFolder(string folderName, bool recursive){}

    public void DeleteNote(string folderName, string noteTitle){}

    public void UpdateNote(string folderName, NoteRecord record){}
}
