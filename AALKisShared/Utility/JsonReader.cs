using Newtonsoft.Json;

namespace AALKisShared.Utility;

public class JsonReader<T>
{
    public static T? JsonFileToType(string path)
    {
        string? json;
        using(FileStream file = new FileStream(path, FileMode.OpenOrCreate))
        {
            json = file.ReadToString();
        }
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static void TypeToJsonFile(T list, string path)
    {
        string json = JsonConvert.SerializeObject(list);
        using(FileStream file = new FileStream(path, FileMode.OpenOrCreate))
        {
            file.WriteString(json);
        }
    }
}
