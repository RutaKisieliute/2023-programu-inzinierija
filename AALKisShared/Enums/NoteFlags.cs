namespace AALKisShared.Enums;

[Flags]
public enum NoteFlags : int
{
    None = 0b0,
    Archived = 0b1,
    InheritKeywords = 0b10,
    ShareKeywords = 0b100,
    Public = 0b1000
}

