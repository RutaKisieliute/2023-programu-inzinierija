using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable, IComparable<NoteRecord>
{
    public long Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? EditDate { get; set; }

    public NoteFlags? Flags { get; set; }

    public NoteRecord(
            long id = -1,
            string? title = null,
            string? content = null,
            DateTime? editDate = null,
            NoteFlags? flags = null)
    {
        Id = id;
        Title = title;
        Content = content;
        EditDate = editDate;
        Flags = flags;
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public void SetFromJsonString(string json)
    {
        this = JsonConvert.DeserializeObject<NoteRecord>(json);
    }

    public void ToJsonFile(string directoryPath)
    {
        using(var stream = new FileStream($"{directoryPath}/{Title}.json",
                    FileMode.Create, FileAccess.Write))
        {
            stream.WriteJson(this);
        }
    }

    public void SetFromJsonFile(string filePath, bool previewOnly = false)
    {
        using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            this = stream.ReadJson<NoteRecord>();
        }

        if(previewOnly)
        {
            this.Content = null;
        }

        Title = Path.GetFileNameWithoutExtension(filePath);
    }

    public int CompareTo(NoteRecord other)
    {
        // If the Archived flags differ,
        if(((this.Flags ^ other.Flags) & NoteFlags.Archived) == NoteFlags.Archived)
        {
            // If this is marked for deletion,
            if((this.Flags & NoteFlags.Archived) == NoteFlags.Archived)
                // Then this should go after
                return 1;
            // Otherwise this should go before
            return -1;
        }
        // Otherwise, fall back to title string comparison
        return this.Title.CompareTo(other.Title);
    }
}

