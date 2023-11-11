namespace AALKisShared;

public interface IJsonSerializable
{
    public string ToJsonString();

    public void SetFromJsonString(string json);
}
