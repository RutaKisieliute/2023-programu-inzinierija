using AALKisAPI.Data;

using Microsoft.EntityFrameworkCore;

using Note = AALKisShared.Records.Note;
using NoteEntity = AALKisAPI.Models.Note;

namespace AALKisAPI.Services;

public class EFNotesRepository : INotesRepository
{
    private readonly NoteDB _database;

    private readonly ITagsRepository _tagsRepository;
    
    public EFNotesRepository(NoteDB database, ITagsRepository tagsRepository)
    {
        _database = database;
        _tagsRepository = tagsRepository;
    }
    
    public IEnumerable<Note> GetAllNotes()
    {
        List<Note> list = new();
        foreach(NoteEntity note in _database.Notes.Include(o => o.Tags))
        {
            list.Add(ToSharedNote(note));
        }
        return list;
    }
    
    public Note GetNote(int id, bool previewOnly)
    {
        var note = _database.Notes.Include(o => o.Tags).First(n => n.Id == id);
        return note != null ? ToSharedNote(note) : new Note(); 
    }

    public bool CheckIfNoteExists(int id)
    {
        return _database.Notes.Find(id) != null;
    }

    public int? CreateNote(int folderId, string noteTitle)
    {
        NoteEntity note = new NoteEntity(){
            Title = noteTitle,
            Flags = 8,
            Content = "",
            FolderId = folderId,
            Modified = DateTime.UtcNow
        };
        _database.Notes.Add(note);
        _database.SaveChanges();
        OnNoteCreation(ToSharedNote(note));
        return note.Id;
    }

    public void DeleteNote(int id)
    {
        var note = _database.Notes.Find(id);
        if(note != null)
        {   
            _database.Notes.Remove(note);
            _database.SaveChanges();
        }
    }

    public void UpdateNote(Note record, int folderId = -1)
    {
        NoteEntity? note = _database.Notes.Find(record.Id);
        if(note == null) throw new Exception();
        note.Title = record.Title;
        note.Content = record.Content;
        note.Flags = (sbyte?) record.Flags;
        if(folderId != -1) note.Folder = _database.Folders.Find(folderId);
        note.Modified = DateTime.UtcNow;
        if(record.Tags != null)
        {
            foreach(string tagDiff in record.Tags)
            {
                if(tagDiff.StartsWith("--")) _tagsRepository.DeleteTag(tagDiff.Remove(0, 2), note.Id);
                else if(tagDiff.StartsWith("++")) _tagsRepository.AddTag(tagDiff.Remove(0, 2), note.Id);
            }
        }  
        _database.SaveChanges();
    }

    public static Note ToSharedNote(NoteEntity note)
    {
        return new Note(){
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Flags = (AALKisShared.Enums.NoteFlags?) note.Flags,
            EditDate = note.Modified,
            Tags = note.Tags.Select(t => t.Tag1).ToList()
        };
    }

    public event EventHandler<Note> NoteCreated;

    protected virtual void OnNoteCreation(Note arg)
    {
        NoteCreated?.Invoke(this, arg);
    }

    public IEnumerable<Note> SearchNotes(string query)
    {
        var list1 = _database.Notes
        .Where(note => note.Title.Contains(query))
        .Select(note => ToSharedNote(note))
        .ToList();

        var list2 = _database.Notes
        .Where(note => note.Content.Contains(query))
        .Select(note => ToSharedNote(note))
        .ToList();

        list1.AddRange(list2);
        var list = list1.Distinct().ToList();

        return list;
    }
}
