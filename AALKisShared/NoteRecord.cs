using AALKisShared.Utility;
using Newtonsoft.Json;

namespace AALKisShared;

public record struct NoteRecord : IJsonSerializable
{
    public long Id { get; set; } = -1;

    public string Title { get; set; } = "";

    public string? Content { get; set; } = null;

    public DateTime EditDate { get; set; } = DateTime.Now;

    [Flags]
    public enum NoteFlags : int
    {
        None = 0b0,
        MarkedForDeletion = 0b1,
        InheritKeywords = 0b10,
        ShareKeywords = 0b100,
        Public = 0b1000
    }

    public NoteFlags Flags { get; set; } = NoteFlags.None;

    public NoteRecord() { }

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
        using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            this = stream.ReadJson<NoteRecord>();
        }

        if(previewOnly)
        {
            this.Content = null;
        }

        Title = Path.GetFileNameWithoutExtension(filePath);
    }
}

