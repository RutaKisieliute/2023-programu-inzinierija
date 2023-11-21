using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface IKeywordsRepository
{
    public delegate bool FilterPredicate(AALKisAPI.Models.Keyword entity, object a);

    public Keyword GetKeyword(string name, int noteId);

    public bool CheckIfKeywordExists(string name);

    public void CreateKeyword(string name, int noteId);

    public void DeleteKeyword(string name, int noteId);

    public IEnumerable<Keyword> Filter(FilterPredicate predicate, object a);
}
