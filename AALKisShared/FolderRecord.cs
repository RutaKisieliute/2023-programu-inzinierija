namespace AALKisShared;

public record class FolderRecord<T> : IComparable<FolderRecord<T>> where T : IJsonSerializable, new()
{
    public int Id { get; set; } = -1;

    public string Name { get; set; } = "";

    public List<T> Records { get; set; } = new List<T>();

    public FolderRecord() { }

    public int CompareTo(FolderRecord<T>? other)
    {
        if(other == null)
            return -1;
        return this.Name.CompareTo(other.Name);
    }
}

