using AALKisShared.Interfaces;

namespace AALKisShared.Records;

public record struct Tag : IJsonSerializable
{
    public string? Name { get; set; } = null;

    public int? NoteCount { get; set; } = null;

    public Tag() { }

    public bool IsValid()
    {
        return Name != null && NoteCount != null;
    }
}
