using AALKisAPI.Services;
using AALKisAPI.Utility;
using Microsoft.AspNetCore.Builder;
public class Program
{
    public static readonly string LogFileName = "./AALKisAPI.log";

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Program>();
                webBuilder.UseEnvironment("Development");
            });

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IFoldersService, FolderRepository>();
        services.AddScoped<INotesService, NoteRepository>();
        services.AddLogging(loggingBuilder => loggingBuilder.AddFile(LogFileName, append: false));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
