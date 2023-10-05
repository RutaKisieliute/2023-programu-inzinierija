using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord
{
    public string Name { get; set; }

    public string Text { get; set; }

    public static NoteRecord FromJsonFile(string path, bool readContents = true)
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException("NoteRecord not found", path);
        }

        NoteRecord record = readContents
                ? JsonFileReaderWriter<NoteRecord>.JsonFileToType(path)
                : new NoteRecord();

        record.Name = Path.GetFileNameWithoutExtension(path);
        return record;
    }

    public void SaveToJson(string directory)
    {
        JsonFileReaderWriter<NoteRecord>.TypeToJsonFile(this, $"{directory}/{this.Name}.json");
    }
}

