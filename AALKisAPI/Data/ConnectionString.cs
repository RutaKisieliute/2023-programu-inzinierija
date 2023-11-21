namespace AALKisAPI.Data;

// This class purpose is to be passed as dependency injection
public record class ConnectionString
{
    public string? Value { get; set; } = null;
    public ConnectionString() { }
}

