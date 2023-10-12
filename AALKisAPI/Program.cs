using AALKisAPI.Services;

namespace AALKisAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IRecordsService, FileSystemRecordsService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
#if DEBUG
        app.UseSwagger();
        app.UseSwaggerUI();
#endif

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
