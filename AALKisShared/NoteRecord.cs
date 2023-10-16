using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable, IComparable<NoteRecord>
{
    public long Id { get; set; } = -1;

    public string? Title { get; set; } = null;

    public string? Content { get; set; } = null;

    public DateTime? EditDate { get; set; } = null;

    [Flags]
    public enum NoteFlags : int
    {
        None = 0b0,
        Archived = 0b1,
        InheritKeywords = 0b10,
        ShareKeywords = 0b100,
        Public = 0b1000
    }

    public NoteFlags? Flags { get; set; } = null;

    public NoteRecord() { }

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

