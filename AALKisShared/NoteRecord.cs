namespace AALKisShared;

public record struct NoteRecord
{
    public string Id { get; set; } = "";

    public string Contents { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.MinValue;

    public NoteRecord() {
        Date = DateTime.Now;
    }

    public NoteRecord(string id, string contents) : this()
    {
        Id = id;
        Contents = contents;
    }
}
