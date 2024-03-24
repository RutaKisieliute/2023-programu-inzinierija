using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface ITagsRepository
{
    public IEnumerable<Tag> GetAllTags(int userId);

    public Tag GetTag(string name);

    public IEnumerable<Tag> GetTagsByNote(int noteId);

    public bool CheckIfTagExists(string name);

    public void AddTag(string name, int noteId);

    public void DeleteTag(string name, int noteId);

    public void DeleteTagsForNote(int noteId);
}
