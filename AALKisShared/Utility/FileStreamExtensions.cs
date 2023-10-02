using System.Text;

namespace AALKisShared.Utility;

public static class FileStreamExtensions
{
    public static string ReadToString(this FileStream file)
    {
        byte[] contents = new byte[(int)file.Length];
        file.Read(contents, 0, (int)file.Length);
        return Encoding.ASCII.GetString(contents);
    }

    public static void WriteString(this FileStream file, string str)
    {
        byte[] contents = Encoding.ASCII.GetBytes(str);
        file.Write(contents, 0, contents.Length);
    }
}
