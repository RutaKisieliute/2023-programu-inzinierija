using System.Text;
using AALKisAPI.Services;

using AALKisShared;
using AALKisShared.Utility;
using AALKisShared.Enums;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class FolderRepository : IFolderRecordsService
{
    private readonly string DBConnection;

    public FolderRepository()
    {
        DBConnection = File.ReadAllText("./Services/databaselogin.txt");
    }

    public List<FolderRecord<NoteRecord>> GetAllFolders(bool previewOnly)
    {
        List<FolderRecord<NoteRecord>> folders = new List<FolderRecord<NoteRecord>>();
        string query = "SELECT id FROM  folders";
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
                    folders.Add(GetFolder(Convert.ToInt32(reader["id"]), false));
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

    public FolderRecord<NoteRecord> GetFolder(int id, bool previewOnly)
    {
        string query = $"SELECT notes.*, folders.title AS ftitle FROM folders, notes WHERE folders.id = {id} AND notes.folder_id = folders.id";
        string query2 = $"SELECT title FROM folders WHERE id = {id}";
        NoteRecord note;
        FolderRecord<NoteRecord> folder = new FolderRecord<NoteRecord>(){Id = id};
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    folder.Name = reader["ftitle"].ToString() ??  "";
                    note = new NoteRecord
                    {
                        Id = Convert.ToInt64(reader["id"]),
                        Title = reader["title"].ToString() ?? ""
                    };
                    if(previewOnly) note.Content = "";
                    else note.Content = reader["content"].ToString();
                    note.Flags = (NoteFlags) Convert.ToInt32(reader["flags"]);
                    folder.Records.Add(note);
                    while(reader.Read())
                    {
                        note = new NoteRecord();
                        note.Id = Convert.ToInt64(reader["id"]);
                        note.Title = reader["title"].ToString() ?? "";
                        note.Content = reader["content"].ToString();
                        note.Flags = (NoteFlags) Convert.ToInt32(reader["flags"]);                        
                        folder.Records.Add(note);
                    }
                }
            }
            if(folder.Name == "")
            {
                using (MySqlCommand cmd2 = new MySqlCommand(query2, connection))
                using(MySqlDataReader reader2 = cmd2.ExecuteReader())
                {
                    reader2.Read();
                    folder.Name = reader2["title"].ToString() ?? "";
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

    public bool CheckIfFolderExists(int id)
    {
        string query = $"SELECT title FROM folders WHERE id = {id}";
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

    public void DeleteFolder(int id, bool force)
    {
        if(!CheckIfFolderExists(id)) return;
        string query1 = $"DELETE FROM folders WHERE id = {id}";
        string query2;
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            connection.Open();
            using(MySqlCommand cmd = new MySqlCommand(query1, connection))
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                cmd.ExecuteNonQuery();
            }
            query2 = $"DELETE FROM notes WHERE folder_id = {id}";
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while deleting folder: " + e.Message);
        }
    }

    public void RenameFolder(string oldFolderName, string newFolderName)
    {
        try
        {
            string query = $"UPDATE folders SET title = '{newFolderName}' WHERE title = '{oldFolderName}'";
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
