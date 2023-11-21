using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface IKeywordsRepository
{
    public IEnumerable<Keyword> GetAllKeywordsByName(string name);

    public Keyword GetKeyword(string name, int noteId);

    public bool CheckIfKeywordExists(string name);

    public void CreateKeyword(string name, int noteId);

    public void DeleteKeyword(string name, int noteId);
}
