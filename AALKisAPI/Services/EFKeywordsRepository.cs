using AALKisAPI.Data;
using AALKisAPI.Models;
using AALKisShared.Records;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
}
