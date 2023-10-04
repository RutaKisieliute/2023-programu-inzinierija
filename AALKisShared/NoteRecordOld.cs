namespace AALKisShared;

public record struct NoteRecordOld
{
    public string Id { get; set; } = "";

    public string Contents { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.MinValue;

    public NoteRecordOld() {
        Date = DateTime.Now;
    }

    public NoteRecordOld(string id, string contents) : this()
    {
        Id = id;
        Contents = contents;
    }
}
