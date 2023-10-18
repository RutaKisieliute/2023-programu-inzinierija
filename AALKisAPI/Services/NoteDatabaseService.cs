using System.Text;
using AALKisAPI.Services;

using AALKisShared;
using AALKisShared.Utility;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class NoteDatabaseService : INoteRecordsService
{
    private readonly IFolderRecordsService _folderService;

    private readonly string DBConnection;

    public enum COLUMN
    {
        Name = 0,
        User,
        Content
    }
    
    public NoteDatabaseService(IFolderRecordsService folderService)
    {
        _folderService = folderService;
        DBConnection = File.ReadAllText("./Services/databaselogin.txt");
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
            note.Flags = (NoteFlags) Convert.ToInt32(reader["public"]);
            return note;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while getting note: " + e.Message);
            throw;
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

    public void CreateNote(string folderName, string noteTitle)
    {
        if(!_folderService.CheckIfFolderExists(folderName)) return;
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
            query3 = $"INSERT INTO notes (id, title, public, content, folder_id) VALUES ({id}, '{noteTitle}', 0, '', {FolderId})";
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
        //if(!CheckIfNoteExists(folderName, record.Title)) return;
        try
        {
            FolderRecord<NoteRecord> folder = _folderService.GetFolder(folderName, false);
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
                    Flags = (NoteFlags) Convert.ToInt32(reader["public"])};
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
}
