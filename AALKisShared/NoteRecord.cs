using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    [Flags]
    public enum NoteFlags : int
    {
        None = 0b0,
        MarkedForDeletion = 0b1,
        InheritKeywords = 0b10,
        ShareKeywords = 0b100,
        Public = 0b1000
    }

    public NoteFlags Flags { get; set; }

    public NoteRecord()
    {
        Id = -1;
        Title = "";
        Content = "";
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
        using(var stream = new FileStream($"{directoryPath}/{Title}.json",
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

        Title = Path.GetFileNameWithoutExtension(filePath);
    }
}

