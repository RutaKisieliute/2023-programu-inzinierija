using System.Text;
using AALKisAPI.Services;

using AALKisShared;
using AALKisShared.Enums;
using AALKisShared.Exceptions;

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AALKisAPI.Utility;

public class NoteRepository : INotesService
{
    private readonly IFoldersService _folderService;

    private readonly string DBConnection;

    public NoteRepository(IFoldersService folderService, string? databaseConnectionString = null)
    {
        _folderService = folderService;
        DBConnection = databaseConnectionString ?? File.ReadAllText("./Services/databaselogin.txt");
    }

    public Note GetNote(int id, bool previewOnly)
    {
        if(!CheckIfNoteExists(id)) throw new NoteException($"Note with id={id} doesn't exist");
        string query = $"SELECT notes.* FROM notes WHERE id = '{id}'";
        Note note = new Note();
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            note.Id = Convert.ToInt64(reader["id"]);
            note.Title = reader["title"].ToString() ?? "";
            if(previewOnly) note.Content = "";
            else note.Content = reader["content"].ToString();
            note.Flags = (NoteFlags) Convert.ToInt32(reader["flags"]);
            return note;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while getting note: " + e.Message);
            throw;
        }
    }

    public bool CheckIfNoteExists(int id)
    {
        string query = $"SELECT notes.id FROM notes WHERE id = '{id}'";
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

    public int CreateNote(int folderId, string noteTitle)
    {
        if(!_folderService.CheckIfFolderExists(folderId)) return -1;
        string query1 = "SELECT MAX(id) AS max FROM notes";
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
            query2 = $"INSERT INTO notes (id, title, flags, content, folder_id) VALUES ({id}, '{noteTitle}', 8, '', {folderId})";
            using(MySqlCommand cmd = new MySqlCommand(query2, connection))
            {
                cmd.ExecuteNonQuery();
            }
            return id;
        }
        catch(Exception e)
        {
            Console.WriteLine("Error while creating note: " + e.ToString());
            return -1;
        }
    }

    public void DeleteNote(int id)
    {
        try
        {
            string query = $"DELETE FROM notes WHERE id = {id}";
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

    public void UpdateNote(Note record, int folderId = -1)
    {
        //if(!CheckIfNoteExists(folderName, record.Title)) return;
        //Folder<Note> folder = _folderService.GetFolder()
        string query = $"UPDATE notes SET content = '{record.Content}',title = '{record.Title}', " +
        $"flags = {(int) (record.Flags ?? 0)}{(folderId == -1 ? "" : ", folder_id = " + folderId)} WHERE id = {record.Id}";
        try
        {
            using(MySqlConnection connection = new MySqlConnection(DBConnection))
            using(MySqlCommand cmd = new MySqlCommand(query, connection))
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

    public void UpdateNote(Note record)
    {

    }


    public List<Note> SearchByTitle(string searchQuery)
    {
        List<Note> list = new List<Note>();
        Note note;
        string query = $"SELECT * FROM notes WHERE UPPER(title) LIKE UPPER('%{searchQuery}%')";
        try
        {
            using MySqlConnection connection = new MySqlConnection(DBConnection);
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();
            connection.Open();
            while(reader.Read())
            {
                note = new Note(){
                    Id = Convert.ToInt64(reader["id"]),
                    Title = reader["title"].ToString() ?? "",
                    Content = reader["content"].ToString(),
                    Flags = (NoteFlags) Convert.ToInt32(reader["flags"])};
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
