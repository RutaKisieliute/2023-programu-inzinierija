using Newtonsoft.Json;
using System.Text.RegularExpressions;
using AALKisShared.Enums;

namespace AALKisShared;

public record struct Note : IJsonSerializable, IComparable<Note>
{
    public int Id { get; set; } = -1;

    public string? Title { get; set; } = null;

    public string? Content { get; set; } = null;

    public DateTime? EditDate { get; set; } = null;

    public NoteFlags? Flags { get; set; } = null;

    public Note() { }

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
                && IsContentValid()
                && EditDate != null
                && Flags != null;
    }

    public bool IsTitleValid()
    {
        // No characters that break links and no funny bytes
        // Only one is needed to invalidate the entire title
        Regex validationRegex = new Regex("[#.\\/\\\\<>?\0-\x1F\x7F-\xFF]{1}");

        return !String.IsNullOrWhiteSpace(Title)
                && !validationRegex.IsMatch(Title);
    }

    public bool IsContentValid()
    {
        // No funny characters
        Regex validationRegex = new Regex("[\0-\x09\x0B-\x1F\x7F-\xFF]{1}");

        return Content != null
            && !validationRegex.IsMatch(Content);
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public void SetFromJsonString(string json)
    {
        this = JsonConvert.DeserializeObject<Note>(json);
    }

    public int CompareTo(Note other)
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
        return (this.Title ?? "").CompareTo(other.Title);
    }
}

