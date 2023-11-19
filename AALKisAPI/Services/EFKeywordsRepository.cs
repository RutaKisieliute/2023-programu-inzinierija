using AALKisAPI.Data;
using AALKisAPI.Models;

namespace AALKisAPI.Services;

public class EFKeywordsRepository : IKeywordsRepository
{
    private readonly NoteDB _database;
    
    public EFKeywordsRepository(NoteDB database)
    {
        _database = database;
    }
    
    public IEnumerable<AALKisAPI.Models.Keyword> GetAllKeywordsByName(string name)
    {
        return _database.Keywords.Where(a => a.Name == name).AsEnumerable();
    }

    public Keyword GetKeyword(string name, int noteId)
    {
        return _database.Keywords.Find(name, noteId);
    }

    public bool CheckIfKeywordExists(string name)
    {
        return _database.Keywords.Where(a => a.Name == name).Any();
    }

    public void CreateKeyword(string name, int noteId)
    {
        _database.Keywords.Add(new Keyword(){
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
}
