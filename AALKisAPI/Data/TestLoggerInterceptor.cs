using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AALKisAPI.Data;

public class TestLoggerInterceptor : SaveChangesInterceptor
{
    public TestLoggerInterceptor() 
    {
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        return new ValueTask<int>(Task.FromResult(result));
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return result;
    }
}

