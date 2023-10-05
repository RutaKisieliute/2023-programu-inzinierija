namespace AALKisShared;

public record class NoteCategoryRecord
{
    public string Name { get; set; }

    public List<NoteRecord> Notes { get; set; }

    public static NoteCategoryRecord FromDirectory(string path)
    {
        string[] splitPath = path.Split(new char[] {'\\', '/'});
        return new NoteCategoryRecord {
            Name = splitPath[splitPath.Length - 1],
            Notes = (from filePath in Directory.GetFiles(path)
                    select NoteRecord.FromFile(filePath)).ToList()};
    }
}

