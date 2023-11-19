using AALKisShared.Interfaces;

namespace AALKisShared.Records;

public record struct Keyword : IJsonSerializable
{
    public string? Name { get; set; } = null;

    public Note? Origin { get; set; } = null;

    public Keyword() { }

    public bool IsValid()
    {
        return Name != null && Origin != null;
    }
}
