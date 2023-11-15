using AALKisAPI.Data;
using AALKisAPI.Services;
using AALKisAPI.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace AALKisAPI;

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
        var _dbConnection = File.ReadAllText("./Services/databaselogin.txt");
        var serverVersion = new MySqlServerVersion("5.2.9");
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        //services.AddScoped<NoteDB>();
        services.AddDbContext<NoteDB>(
        dbContextOptions => dbContextOptions
            .UseMySql(_dbConnection, serverVersion)                
        );
        services.AddScoped<IFoldersRepository, EFFoldersRepository>();
        services.AddScoped<INotesRepository, EFNotesRepository>();
        services.AddLogging(loggingBuilder => loggingBuilder.AddFile(LogFileName, append: false));
        /*services.AddDbContext<Models.Database>(options => {
            var connectionString = File.ReadAllText("./Services/databaselogin.txt");
            options.UseSqlServer(connectionString);
        });*/
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
