using AALKisShared.Interfaces;

namespace AALKisShared.Records;

public record class Folder<T> : IComparable<Folder<T>>, IJsonSerializable
{
    public int Id { get; set; } = -1;

    public string Name { get; set; } = "";

    public List<T> Records { get; set; } = new List<T>();

    public Folder() { }

    public int CompareTo(Folder<T>? other)
    {
        if(other == null)
        {
            return -1;
        }
        return this.Name.CompareTo(other.Name);
    }
}

