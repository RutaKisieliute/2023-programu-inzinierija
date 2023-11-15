using AALKisAPI.Data;
using AALKisAPI.Models;
using AALKisShared;
using Microsoft.EntityFrameworkCore;

namespace AALKisAPI.Services;

public class EFNotesService : INotesService
{
    private readonly NoteDB _database;
    
    public EFNotesService(NoteDB database)
    {
        _database = database;
    }
    
    public AALKisShared.Note GetNote(int id, bool previewOnly)
    {
        Console.WriteLine("The time now is:"+ DateTime.Now+"IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        var note = _database.Notes.Find(id);
        Console.WriteLine("The note's time is:"+ note.Modified +"IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        return note != null ? ToSharedNote(note) : new AALKisShared.Note(); 
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
        int ret = _database.Notes.Add(note).Entity.Id;
        _database.SaveChanges();
        return ret;
    }

    public void DeleteNote(int id)
    {
        _database.Notes.Remove(_database.Notes.Find(id));
        _database.SaveChanges();
    }

    public void UpdateNote(AALKisShared.Note record, int folderId = -1)
    {
        Models.Note note = _database.Notes.Find(record.Id);
        if(note == null) throw new Exception();
        note.Title = record.Title;
        note.Content = record.Content;
        note.Flags = (sbyte?) record.Flags;
        if(folderId != -1) note.Folder = _database.Folders.Find(folderId);
        note.Modified = DateTime.UtcNow;
        Console.WriteLine("The modification time is:"+ note.Modified+"IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        _database.SaveChanges();
    }

    public List<AALKisShared.Note> SearchByTitle(string searchQuery)
    {
        var list1 = (List<Models.Note>) _database.Notes.Where(note => note.Title.Contains(searchQuery));
        var list2 = new List<AALKisShared.Note>();
        foreach(Models.Note note in list1)
        {
            list2.Add(ToSharedNote(note));
        }
        return list2;
    }

    public static AALKisShared.Note ToSharedNote(AALKisAPI.Models.Note note)
    {
        return new AALKisShared.Note(){
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Flags = (AALKisShared.Enums.NoteFlags?) note.Flags,
            EditDate = note.Modified
        };
    }
}
