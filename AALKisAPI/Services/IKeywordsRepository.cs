using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface IKeywordsRepository
{
    public IEnumerable<Keyword> GetAllKeywords(int userId);
    
    public IEnumerable<Keyword> GetAllKeywordsByFolder(int folderId);

    public IEnumerable<Keyword> GetAllKeywordsByNote(int noteId);

    public Keyword GetKeyword(string name, int noteId);

    public bool CheckIfKeywordExists(string name);

    public void CreateKeyword(string name, int noteId);

    public void DeleteKeyword(string name, int noteId);

    public void CreateKeywordsForNote(IEnumerable<string> keywordNames, int noteId);

    public void DeleteKeywordsForNote(IEnumerable<string> keywordNames, int noteId);

    public void UpdateKeywordsForNote(IEnumerable<string> keywordNames, int noteId);

}
