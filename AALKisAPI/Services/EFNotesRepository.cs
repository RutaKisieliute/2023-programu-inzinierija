using AALKisAPI.Data;
using AALKisAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AALKisAPI.Services;

public class EFNotesRepository : INotesRepository
{
    private readonly NoteDB _database;
    
    public EFNotesRepository(NoteDB database)
    {
        _database = database;
    }
    
    public AALKisShared.Records.Note GetNote(int id, bool previewOnly)
    {
        var note = _database.Notes.Find(id);
        return note != null ? ToSharedNote(note) : new AALKisShared.Records.Note(); 
    }

    public bool CheckIfNoteExists(int id)
    {
        return _database.Notes.Find(id) != null;
    }

    public int? CreateNote(int folderId, string noteTitle)
    {
        Models.Note note = new Models.Note(){
            Title = noteTitle,
            Flags = 8,
            Content = "",
            FolderId = folderId,
            Modified = DateTime.UtcNow
        };
        _database.Notes.Add(note);
        _database.SaveChanges();
        return note.Id;
    }

    public void DeleteNote(int id)
    {
        _database.Notes.Remove(_database.Notes.Find(id));
        _database.SaveChanges();
    }

    public void UpdateNote(AALKisShared.Records.Note record, int folderId = -1)
    {
        Models.Note note = _database.Notes.Find(record.Id);
        if(note == null) throw new Exception();
        note.Title = record.Title;
        note.Content = record.Content;
        note.Flags = (sbyte?) record.Flags;
        if(folderId != -1) note.Folder = _database.Folders.Find(folderId);
        note.Modified = DateTime.UtcNow;
        _database.SaveChanges();
    }

    public List<AALKisShared.Records.Note> SearchByTitle(string searchQuery)
    {
        var list1 = (List<Models.Note>) _database.Notes.Where(note => note.Title.Contains(searchQuery));
        var list2 = new List<AALKisShared.Records.Note>();
        foreach(Models.Note note in list1)
        {
            list2.Add(ToSharedNote(note));
        }
        return list2;
    }

    public static AALKisShared.Records.Note ToSharedNote(AALKisAPI.Models.Note note)
    {
        return new AALKisShared.Records.Note(){
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Flags = (AALKisShared.Enums.NoteFlags?) note.Flags,
            EditDate = note.Modified
        };
    }
}
