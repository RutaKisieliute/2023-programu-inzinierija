using AALKisShared.Records;

namespace AALKisAPI.Services;

public interface ITagsRepository
{
    public IEnumerable<Tag> GetAllTags();

    public Tag GetTag(string name);

    public bool CheckIfTagExists(string name);

    public void AddTag(string name, int noteId);

    public void DeleteTag(string name, int noteId);
}
