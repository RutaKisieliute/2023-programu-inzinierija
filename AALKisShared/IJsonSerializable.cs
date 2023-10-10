namespace AALKisShared;

public interface IJsonSerializable
{
    public string ToJsonString();

    public void SetFromJsonString(string json);

    public void ToJsonFile(string directoryPath);

    public void SetFromJsonFile(string filePath, bool previewOnly);
}
