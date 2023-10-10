using System.Text;
using Newtonsoft.Json;

namespace AALKisShared.Utility;

public static class FileStreamExtensions
{
    public static string ReadString(this FileStream file)
    {
        byte[] contents = new byte[(int)file.Length];
        file.Read(contents, 0, (int)file.Length);
        return Encoding.UTF8.GetString(contents);
    }

    public static void WriteString(this FileStream file, string str)
    {
        byte[] contents = Encoding.UTF8.GetBytes(str);
        file.Write(contents, 0, contents.Length);
        file.Flush();
    }

    public static T ReadJson<T>(this FileStream file)
    {
        string json = file.ReadString();
        return JsonConvert.DeserializeObject<T>(json)
            ?? throw new JsonSerializationException($"Failed to deserialize JSON from {file}");
    }

    public static void WriteJson<T>(this FileStream file, T obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        file.WriteString(json);
        return;
    }
}
