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
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd1 = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader = cmd1.ExecuteReader())
            {
                reader.Read();
                folder.Id = Convert.ToInt32(reader["id"]);
            }
            using (MySqlCommand cmd2 = new MySqlCommand(query2, connection))
            using(MySqlDataReader reader = cmd2.ExecuteReader())
            {
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
        catch(Exception e)
        {
            //backup?
            Console.WriteLine("This fucked up in GetFolder -->" + e.Message + "!!!");
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

    public void CreateFolder(string folderName)
    {
        string query1 = "SELECT MAX(id) AS max FROM folders";
        string query2;
        int id;
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                
                reader.Read();
                id = Convert.ToInt32(reader["max"]) + 1;
            }
            query2 = $"INSERT INTO folders (id, title, user_id) VALUES ({id}, '{folderName}', 1)";
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(cmd.ExecuteNonQuery() < 1) Console.WriteLine("nothing changed||||||||");
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void CreateNote(string folderName, string noteTitle)
    {
        if(!CheckIfFolderExists(folderName)) return;
        string query1 = "SELECT MAX(id) AS max FROM notes";
        string query2 = $"SELECT id FROM folders WHERE title = '{folderName}'";
        string query3;
        int id, FolderId;
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                id = Convert.ToInt32(reader["max"]) + 1;
            }
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                FolderId = Convert.ToInt32(reader["id"]);
            }
            query3 = $"INSERT INTO notes (id, title, public, content, folder_id) VALUES ({id}, '{noteTitle}', true, '', {FolderId})";
            using(MySqlCommand cmd = new MySqlCommand(query3, connection))
            {
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("should be good!!!!!!!!!!!!!!?????????????!!!!!!!!");
            return;
        }
        catch(Exception e)
        {
            Console.WriteLine("fucked in create ---->" + e.ToString() + "!!!!!!!!!!!!");
            return;
        }
    }

    public void DeleteFolder(string folderName, bool recursive)
    {
        if(!CheckIfFolderExists(folderName)) return;
        string query1 = $"SELECT id FROM folders WHERE title = '{folderName}'";
        string query2, query3;
        MySqlDataReader reader;
        int id;
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd = new MySqlCommand(query1, connection))
            {
                reader = cmd.ExecuteReader();
                reader.Read();
                id = Convert.ToInt32(reader["id"]);
            }
            query2 = $"DELETE FROM folders WHERE id = {id}";
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            {
                cmd.ExecuteNonQuery();
            }
            if(recursive)
            {
                query3 = $"DELETE FROM notes WHERE folder_id = {id}";
                using(MySqlCommand cmd = new MySqlCommand(query3, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void DeleteNote(string folderName, string noteTitle)
    {
        try
        {
            NoteRecord note = GetNote(folderName, noteTitle, false);
            string query = $"DELETE FROM notes WHERE id = {note.Id}";
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            {
                cmd.ExecuteNonQuery();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void UpdateNote(string folderName, NoteRecord record)
    {
        if(!CheckIfNoteExists(folderName, record.Title)) return;
        bool IsPublic = ((record.Flags & NoteRecord.NoteFlags.Public) == NoteRecord.NoteFlags.Public);
        try
        {
            FolderRecord<NoteRecord> folder = GetFolder(folderName, false);
            string query = $"UPDATE notes SET content = '{record.Content}', title = '{record.Title}', public = {IsPublic}, folder_id = {folder.Id} WHERE id = {record.Id}";
            using (MySqlConnection connection = new MySqlConnection(DBConnection))
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
