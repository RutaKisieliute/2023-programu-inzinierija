using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AALKisAPI.Data;

public class LoggerInterceptor : SaveChangesInterceptor
{
    public string FilePath { get; set; }
    public LoggerInterceptor(string directoryPath) 
    {
        FilePath = directoryPath + "/" + DateTime.UtcNow.Year.ToString() + "-" + DateTime.UtcNow.Month.ToString() + "-" + DateTime.UtcNow.Day.ToString() + "databaselog.txt";
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        File.AppendAllTextAsync(FilePath, eventData.ToString() + "\n", cancellationToken);
        return new ValueTask<int>(Task.FromResult(result));
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        File.AppendAllText(FilePath, DateTime.UtcNow + " " + eventData.ToString() + "\n");
        return result;
    }
}

