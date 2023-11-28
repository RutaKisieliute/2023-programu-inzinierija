using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface IKeywordsRepository
{
    public IEnumerable<Keyword> GetAllKeywords();
    
    public IEnumerable<Keyword> GetAllKeywordsByName(string name);

    public IEnumerable<Keyword> GetAllKeywordsByFolder(int folderId);

    public IEnumerable<Keyword> GetAllKeywordsByNote(int noteId);

    public Keyword GetKeyword(string name, int noteId);

    public bool CheckIfKeywordExists(string name);

    public void CreateKeyword(string name, int noteId);

    public void DeleteKeyword(string name, int noteId);
    void CreateKeywordList(IEnumerable<string> keywordNames);

    void DeleteKeywordList(IEnumerable<string> keywordNames);
}
