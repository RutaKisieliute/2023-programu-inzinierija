using AALKisShared.Utility;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using AALKisShared.Enums;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable, IComparable<NoteRecord>
{
    public long Id { get; set; } = -1;

    public string? Title { get; set; } = null;

    public string? Content { get; set; } = null;

    public DateTime? EditDate { get; set; } = null;

    public NoteFlags? Flags { get; set; } = null;

    public NoteRecord() { }

    public void Update(
            string? title = null,
            string? content = null,
            DateTime? editDate = null,
            NoteFlags? flags = null)
    {
        Title = title ?? Title;
        Content = content ?? Content;
        EditDate = editDate ?? EditDate;
        Flags = flags ?? Flags;
        return;
    }

    public bool IsValid()
    {
        return Id > 0
                && IsTitleValid()
                && Content != null
                && EditDate != null
                && Flags != null;
    }

    public bool IsTitleValid()
    {
        Regex validationRegex = new Regex("[#./\\\n]+");

        return !String.IsNullOrWhiteSpace(Title)
                && !validationRegex.IsMatch(Title);
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

