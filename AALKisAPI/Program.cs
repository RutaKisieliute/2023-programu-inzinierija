using AALKisAPI.Data;
using AALKisAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AALKisAPI;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        if(CurrentEnvironment.EnvironmentName != "Integration Test")
        {
            var _dbConnection = new ConnectionString { Value = File.ReadAllText("./Services/databaselogin.txt") };
            services.AddSingleton<SaveChangesInterceptor>(i => new LoggerInterceptor("./Logs"));
            //services.AddScoped<LoggerInterceptor>();
            services.AddSingleton(_dbConnection);
            services.AddDbContext<NoteDB>();
        }
        else services.AddSingleton<SaveChangesInterceptor>(i => new TestLoggerInterceptor());

        services.AddScoped<IFoldersRepository, EFFoldersRepository>();
        services.AddScoped<INotesRepository, EFNotesRepository>();
        services.AddScoped<IKeywordsRepository, EFKeywordsRepository>();
        services.AddScoped<ITagsRepository, EFTagsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
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
