using Newtonsoft.Json;

namespace AALKisShared.Utility;

public static class JsonFileReaderWriter
{
    public static T? JsonFileToType<T>(string path)
    {
        using(FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            string json = file.ReadToString();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static void TypeToJsonFile<T>(T obj, string path)
    {
        string json = JsonConvert.SerializeObject(obj);
        using(FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            file.WriteString(json);
        }
    }
}
