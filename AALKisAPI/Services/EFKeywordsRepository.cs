using AALKisAPI.Data;
using Microsoft.EntityFrameworkCore;
using Keyword = AALKisShared.Records.Keyword;
using KeywordEntity = AALKisAPI.Models.Keyword;

namespace AALKisAPI.Services;

public class EFKeywordsRepository : IKeywordsRepository
{
    private readonly NoteDB _database;
    
    public EFKeywordsRepository(NoteDB database)
    {
        _database = database;
    }

    public IEnumerable<Keyword> GetAllKeywords()
    {
        var list1 = _database.Keywords.AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }
    
    public IEnumerable<Keyword> GetAllKeywordsByName(string name)
    {
        var list1 = _database.Keywords.Where(a => a.Name == name).AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }

    public IEnumerable<Keyword> GetAllKeywordsByNote(int noteId)
    {
        var list1 = _database.Keywords.Where(a => a.NoteId == noteId).AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
            list2.Add(ToSharedKeyword(entity));
        }
        return list2;
    }

    public IEnumerable<Keyword> GetAllKeywordsByFolder(int folderId)
    {
        var list1 = _database.Keywords.Include(o => o.Note).Where(a => a.Note.FolderId == folderId).AsEnumerable();
        List<Keyword> list2 = new List<Keyword>();
        foreach(KeywordEntity entity in list1)
        {
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

    public void CreateKeywordList(IEnumerable<string> keywordNames)
    {
        foreach (var name in keywordNames)
        {
            _database.Keywords.Add(new KeywordEntity()
            {
                Name = name,
                NoteId = 0
            });
        }

        _database.SaveChanges();
    }

    public void DeleteKeywordList(IEnumerable<string> keywordNames)
    {
        foreach (var name in keywordNames)
        {
            var keywordToRemove = _database.Keywords.FirstOrDefault(k => k.Name == name && k.NoteId == 0);

            if (keywordToRemove != null)
            {
                _database.Keywords.Remove(keywordToRemove);
            }
        }

        _database.SaveChanges();
    }
}
