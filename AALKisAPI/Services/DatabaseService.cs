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
    
    public DatabaseService()
    {
        DBConnection = File.ReadAllText("./Services/databaselogin.txt");
    }

    /*public List<string?> GetTags()
    {
        List<string?> list = new List<string?>();
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT tag FROM tags", connection);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                list.Add(reader["tag"].ToString());
            }
            return list;
        }
        catch(Exception)
        {
            return list;
        }
    }

    public List<NoteRecord> GetNotesByTag(string tag)
    {
        List<NoteRecord> list = new List<NoteRecord>();
        NoteRecord note = new NoteRecord();
        string query = $"SELECT title, note_id FROM tags JOIN notes ON note_id = id WHERE public = true AND tag = '{tag}'";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                note.Id = Convert.ToInt64(reader["id"]);
                note.Title = reader["title"].ToString() ?? "";
                note.Content = reader["content"].ToString();
                if(Convert.ToBoolean(reader["public"])) note.Flags = note.Flags | NoteRecord.NoteFlags.Public;
            }
        }
        catch(Exception)
        {
            return list;
        }
        return list;
    }

    /*public List<string?> GetNote(string id)
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
            Console.WriteLine("Error while getting all folders: " + e.Message);
            return folders;
        }

    }

    public FolderRecord<NoteRecord> GetFolder(string folderName, bool previewOnly)
    {
        //if(!CheckIfFolderExists(folderName)) throw new Exception("The folder doesn't exist");
        string query2 = $"SELECT id FROM folders WHERE title = '{folderName}'";
        string query1 = $"SELECT notes.* FROM folders, notes WHERE folders.title = '{folderName}' AND notes.folder_id = folders.id";
        NoteRecord note;
        FolderRecord<NoteRecord> folder = new FolderRecord<NoteRecord>(){Name = folderName};
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using (MySqlCommand cmd1 = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader1 = cmd1.ExecuteReader())
            {
                if(reader1.Read())
                {
                    folder.Id = Convert.ToInt32(reader1["folder_id"]);
                    note = new NoteRecord();
                    note.Id = Convert.ToInt64(reader1["id"]);
                    note.Title = reader1["title"].ToString() ?? "";
                    note.Content = reader1["content"].ToString();
                    note.Flags = (NoteRecord.NoteFlags) Convert.ToInt32(reader1["public"]);
                    folder.Records.Add(note);
                    while(reader1.Read())
                    {
                        note = new NoteRecord();
                        note.Id = Convert.ToInt64(reader1["id"]);
                        note.Title = reader1["title"].ToString() ?? "";
                        note.Content = reader1["content"].ToString();
                        note.Flags = (NoteRecord.NoteFlags) Convert.ToInt32(reader1["public"]);                        
                        folder.Records.Add(note);
                    }
                }
                else
                {
                    using(MySqlCommand cmd2 = new MySqlCommand(query2, connection))
                    using(MySqlDataReader reader2 = cmd2.ExecuteReader())
                    {
                        reader2.Read();
                        folder.Id = Convert.ToInt32(reader2["id"]);
                    }
                }
            }
            return folder;
        }
        catch(Exception e)
        {
            //backup?
            Console.WriteLine("Error while getting folder: " + e.Message);
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
            note.Flags = (NoteRecord.NoteFlags) Convert.ToInt32(reader["public"]);
            return note;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while getting note: " + e.Message);
            throw;
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
        catch(Exception e)
        {
            Console.WriteLine("Error while checking if folder exists: " + e.Message);
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
        catch(Exception e)
        {
            Console.WriteLine("Error while checking if note exists: " + e.Message);
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
            Console.WriteLine("Error while creating folder: " + e.Message);
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
            return;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while creating note: " + e.ToString());
            return;
        }
    }

    public void DeleteFolder(string folderName, bool recursive)
    {
        if(!CheckIfFolderExists(folderName)) return;
        string query1 = $"SELECT id FROM folders WHERE title = '{folderName}'";
        string query2, query3;
        int id;
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                
                reader.Read();
                id = Convert.ToInt32(reader["id"]);
            }
            query2 = $"DELETE FROM folders WHERE id = {id}";
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
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
            Console.WriteLine("Error while deleting folder: " + e.Message);
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
            Console.WriteLine("Error while deleting note: " + e.Message);
        }
    }

    public void UpdateNote(string folderName, NoteRecord record)
    {
        if(!CheckIfNoteExists(folderName, record.Title)) return;
        try
        {
            FolderRecord<NoteRecord> folder = GetFolder(folderName, false);
            string query = $"UPDATE notes SET content = '{record.Content}', title = '{record.Title}', public = {(int) (record.Flags ?? 0)}, folder_id = {folder.Id} WHERE id = {record.Id}";
            Console.WriteLine(record.Flags);
            using (MySqlConnection connection = new MySqlConnection(DBConnection))
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while updating note: " + e.Message);
        }
    }

    public List<NoteRecord> SearchByTitle(string searchQuery)
    {
        List<NoteRecord> list = new List<NoteRecord>();
        NoteRecord note;
        string query = $"SELECT * FROM notes WHERE UPPER(title) LIKE UPPER('%{searchQuery}%')";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();
            connection.Open();
            while(reader.Read())
            {
                note = new NoteRecord(){
                    Id = Convert.ToInt64(reader["id"]),
                    Title = reader["title"].ToString() ?? "",
                    Content = reader["content"].ToString(),
                    Flags = (NoteRecord.NoteFlags) Convert.ToInt32(reader["public"])};
                list.Add(note);
            }
            return list;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while searching by title: " + e.Message);
            return list;
        }
    }

    public void RenameFolder(string OldFolderName, string NewFolderName)
    {
        try
        {
            string query = $"UPDATE folders SET title = '{NewFolderName}' WHERE title = '{OldFolderName}'";
            using (MySqlConnection connection = new MySqlConnection(DBConnection))
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while renaming folder: " + e.Message);
        }
    }
}
