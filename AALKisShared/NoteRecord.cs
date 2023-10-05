using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord
{
    [JsonIgnore]
    public string Name { get; set; }

    public string Text { get; set; }

    public static NoteRecord FromJsonFile(string path, bool readContents = true)
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException("NoteRecord not found", path);
        }

        NoteRecord record = readContents
                ? JsonFileReaderWriter.JsonFileToType<NoteRecord>(path)
                : new NoteRecord();

        record.Name = Path.GetFileNameWithoutExtension(path);
        return record;
    }

    public static NoteRecord FromJsonString(string json, string name)
    {
        NoteRecord result = JsonConvert.DeserializeObject<NoteRecord>(json);
        result.Name = name;
        return result;
    }

    public void SaveToJsonFile(string directory)
    {
        JsonFileReaderWriter.TypeToJsonFile<NoteRecord>(this, $"{directory}/{this.Name}.json");
    }
}

