using AALKisAPI.Data;
using Tag = AALKisShared.Records.Tag;
using TagEntity = AALKisAPI.Models.Tag;

namespace AALKisAPI.Services;

public class EFTagsRepository : ITagsRepository
{
    private readonly NoteDB _database;
    
    public EFTagsRepository(NoteDB database)
    {
        _database = database;
    }
    
    public IEnumerable<Tag> GetAllTags(int userId)
    {
        var tagList = new List<Tag>();
        int index;
        foreach(TagEntity tag in _database.Tags.Where(t => t.Note.UserId == userId).ToList())
        {
            if((index = tagList.FindIndex(t => t.Name == tag.Tag1)) != -1)
            {
                var found = tagList[index];
                found.NoteCount++;
                tagList[index] = found;
            }
            else
            {
                tagList.Add(new Tag(){Name = tag.Tag1, NoteCount = 1});
            }
        }
        return tagList;
    }

    public Tag GetTag(string name)
    {
        return new Tag(){Name = name, NoteCount = _database.Tags.Where(t => t.Tag1 == name).Count()};
    }

    public IEnumerable<Tag> GetTagsByNote(int noteId)
    {
        return _database.Tags.Where(t => t.NoteId == noteId).Select(t => GetTag(t.Tag1));
    }

    public bool CheckIfTagExists(string name)
    {
        return _database.Tags.Where(t => t.Tag1 == name).Any();
    }

    public void AddTag(string name, int noteId)
    {
        TagEntity tag = new TagEntity(){Tag1 = name, NoteId = noteId};
        _database.Tags.Add(tag);
        _database.SaveChanges();
    }

    public void DeleteTag(string name, int noteId)
    {
        var tag = _database.Tags.Find(noteId, name);
        if(tag != null)
        {   
            _database.Tags.Remove(tag);
            _database.SaveChanges();
        }
    }

    public void DeleteTagsForNote(int noteId)
    {
        var tags = _database.Tags.Where(t => t.NoteId == noteId);
        foreach(TagEntity tag in tags)
        {
            _database.Tags.Remove(tag);
        }
        _database.SaveChanges();
    }
}
