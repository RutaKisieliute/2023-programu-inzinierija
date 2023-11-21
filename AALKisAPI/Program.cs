using AALKisAPI.Data;
using AALKisAPI.Services;
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

    public Program(IWebHostEnvironment env)
    {
        CurrentEnvironment = env;
    }

    private IWebHostEnvironment CurrentEnvironment { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        var isTestDb = "Test" == CurrentEnvironment.EnvironmentName;
        string projectDirectory = CurrentEnvironment.ContentRootPath;
        var _dbConnection = File.ReadAllText(
            projectDirectory +
            (isTestDb
            ?
            "/Services/testdatabaselogin.txt"
            :
            "/Services/databaselogin.txt"
            ));
        var serverVersion = new MySqlServerVersion("5.2.9");
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton(new ConnectionString { Value = _dbConnection });
        //services.AddScoped<NoteDB>();

        services.AddDbContext<NoteDB>(
        dbContextOptions => dbContextOptions
            .UseMySql(_dbConnection, serverVersion)                
        );
        services.AddScoped<IFoldersRepository, EFFoldersRepository>();
        services.AddScoped<INotesRepository, EFNotesRepository>();
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
