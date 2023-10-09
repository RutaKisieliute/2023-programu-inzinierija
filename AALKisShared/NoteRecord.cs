using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable
{
    public string Name { get; set; }

    public string Text { get; set; }

    [Flags]
    public enum NoteFlags
    {
        None = 0b0,
        MarkedForDeletion = 0b1,
        InheritKeywords = 0b10,
        ShareKeywords = 0b100
    }

    public NoteFlags Flags { get; set; }

    public NoteRecord()
    {
        Name = "";
        Text = "";
        Flags = NoteFlags.None;
    }

    public string ToJsonString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public void SetFromJsonString(string json)
    {
        this = JsonConvert.DeserializeObject<NoteRecord>(json);
    }

    public void ToJsonFile(string directoryPath)
    {
        using(var stream = new FileStream($"{directoryPath}/{Name}.json",
                    FileMode.Create, FileAccess.Write))
        {
            stream.WriteJson(this);
        }
    }

    public void SetFromJsonFile(string filePath, bool previewOnly = false)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException("Failed to set NoteRecord from file", filePath);
        }

        if(!previewOnly)
        {
            using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                this = stream.ReadJson<NoteRecord>();
            }
        }

        Name = Path.GetFileNameWithoutExtension(filePath);
    }
}

