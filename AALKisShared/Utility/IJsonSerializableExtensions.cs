using Newtonsoft.Json;
using AALKisShared.Interfaces;

namespace AALKisShared.Utility;

public static class JsonSerializableExtensions
{
    public static string ToJsonString<T>(this T obj) where T : struct, IJsonSerializable
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static void SetFromJsonString<T>(this T obj, string json) where T : struct, IJsonSerializable
    {
        obj = JsonConvert.DeserializeObject<T>(json);
    }
}
