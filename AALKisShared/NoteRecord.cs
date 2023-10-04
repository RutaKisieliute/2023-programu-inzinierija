using AALKisShared.Utility;

namespace AALKisShared;

public record struct NoteRecord
{
    public string Name { get; set; }

    public static NoteRecord FromFile(string path)
    {
        return JsonFileReader<NoteRecord>.JsonFileToType(path);
    }
}

