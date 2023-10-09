using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct KeywordRecord
{
    public string Name { get; set; }

    public string OriginPath { get; set; }

    public static KeywordRecord FromJsonFile(string path, bool readContents = true)
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException("KeywordRecord not found", path);
        }
        KeywordRecord record = readContents
                ? JsonFileReaderWriter.JsonFileToType<KeywordRecord>(path)
                : new KeywordRecord();

        record.Name = Path.GetFileNameWithoutExtension(path);

        return record;
    }

    public void SaveToJsonFile(string directory)
    {
        JsonFileReaderWriter.TypeToJsonFile<KeywordRecord>(this, $"{directory}/{this.Name}.json");
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static KeywordRecord FromJsonString(string json, string? name = null)
    {
        KeywordRecord result = JsonConvert.DeserializeObject<KeywordRecord>(json);
        if(name != null)
        {
            result.Name = name;
        }
        return result;
    }

}
