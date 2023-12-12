using AALKisAPI.Data;
using Microsoft.EntityFrameworkCore;
using Keyword = AALKisShared.Records.Keyword;
using KeywordEntity = AALKisAPI.Models.Keyword;
using NoteModel = AALKisAPI.Models.Note;

namespace AALKisAPI.Services;

public class EFKeywordsRepository : IKeywordsRepository
{
    private readonly NoteDB _database;    
    public EFKeywordsRepository(NoteDB database,
            ILogger<EFKeywordsRepository> logger)
    {
        _database = database;
    }

    public IEnumerable<Keyword> GetAllKeywords()
    {
        var notes = _database.Notes.ToList();

        var list1 = _database.Keywords.AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            entity.Note = notes.FirstOrDefault(note => note.Id == entity.NoteId)
                    ?? throw new Exception($"Keyword {entity.Name} references missing note with id {entity.NoteId}");
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }

    public IEnumerable<Keyword> GetAllKeywordsByNote(int noteId)
    {
        var notes = _database.Notes.ToList();

        var list1 = _database.Keywords
                .Where(a => a.NoteId == noteId)
                .AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            entity.Note = notes.FirstOrDefault(note => note.Id == entity.NoteId)
                    ?? throw new Exception($"Keyword {entity.Name} references missing note with id {entity.NoteId}");
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }

    public IEnumerable<Keyword> GetAllKeywordsByFolder(int folderId)
    {
        var notes = _database.Notes.ToList();

        var list1 = _database.Keywords
                .Include(o => o.Note)
                .Where(a => a.Note.FolderId == folderId)
                .AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            entity.Note = notes.FirstOrDefault(note => note.Id == entity.NoteId)
                    ?? throw new Exception($"Keyword {entity.Name} references missing note with id {entity.NoteId}");
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }
    
    public Keyword GetKeyword(string name, int noteId)
    {
        return ToSharedKeyword(_database.Keywords.Find(name, noteId));
    }

    public bool CheckIfKeywordExists(string name)
    {
        return _database.Keywords.Where(a => a.Name == name).Any();
    }

    public void CreateKeyword(string name, int noteId)
    {
        _database.Keywords.Add(new KeywordEntity(){
            Name = name,
            NoteId = noteId
        });
        _database.SaveChanges();
    }

    public void DeleteKeyword(string name, int noteId)
    {
        var keywordToRemove = _database.Keywords.Find(name, noteId);
        if(keywordToRemove != null)
        {
            _database.Remove(keywordToRemove);
            _database.SaveChanges();
        }
    }
    
    public static Keyword ToSharedKeyword(KeywordEntity entity)
    {
        return new Keyword(){
            Name = entity.Name,
            Origin = EFNotesRepository.ToSharedNote(entity.Note)
        };
    }

    public void CreateKeywordsForNote(IEnumerable<string> keywordNames, int noteId)
    {
        foreach (var name in keywordNames)
        {
            _database.Keywords.Add(new KeywordEntity()
            {
                Name = name,
                NoteId = noteId
            });
        }

        _database.SaveChanges();
    }

    public void DeleteKeywordsForNote(IEnumerable<string> keywordNames, int noteId)
    {
        foreach (var name in keywordNames)
        {
            var keywordToRemove = _database.Keywords
                .FirstOrDefault(k => k.Name == name && k.NoteId == noteId);

            if (keywordToRemove != null)
            {
                _database.Keywords.Remove(keywordToRemove);
            }
        }

        _database.SaveChanges();
    }

    public void UpdateKeywordsForNote(IEnumerable<string> newKeywords, int noteId)
    {
        Console.WriteLine("UpdateKeywordsForNote");
        IEnumerable<string> oldKeywords = GetAllKeywordsByNote(noteId).Select(x => x.Name);
        var commonKeywords = oldKeywords.Intersect(newKeywords);

        var toDeleteKeywords = oldKeywords.Except(commonKeywords);
        var toCreateKeywords = newKeywords.Except(commonKeywords);

        DeleteKeywordsForNote(toDeleteKeywords, noteId);
        CreateKeywordsForNote(toCreateKeywords, noteId);

        _database.SaveChanges();
    }
}
