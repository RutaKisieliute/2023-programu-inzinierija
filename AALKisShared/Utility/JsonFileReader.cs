using Newtonsoft.Json;

namespace AALKisShared.Utility;

public class JsonFileReader<T>
{
    public static T? JsonFileToType(string path)
    {
        using(FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            string json = file.ReadToString();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static void TypeToJsonFile(T list, string path)
    {
        string json = JsonConvert.SerializeObject(list);
        using(FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            file.WriteString(json);
        }
    }
}
