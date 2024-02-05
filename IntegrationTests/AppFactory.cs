using Microsoft.AspNetCore.Mvc.Testing;
using AALKisAPI.Data;
using FolderEntity = AALKisAPI.Models.Folder;
using NoteEntity = AALKisAPI.Models.Note;

namespace IntegrationTests;
public class AppFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    private static ConnectionString _dbConnection = new ConnectionString { Value = File.ReadAllText("./../../../AALKisAPI/Services/testdatabaselogin.txt") };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        builder.ConfigureServices(services =>
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
            });
            services.AddSingleton(_dbConnection);
            services.AddDbContext<NoteDB>();
        });
        /*
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                builder.ConfigureServices(services =>
                {
                    using (var context = services.BuildServiceProvider().GetService<NoteDB>()!)
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.Folders.AddRange(
                                    new FolderEntity
                                    {
                                        Title = "testFolder1",
                                        UserId = 1
                                    },
                                    new FolderEntity
                                    {
                                        Title = "testFolder2",
                                        UserId = 1
                                    }
                                );
                        context.Notes.AddRange(
                                    new NoteEntity
                                    {
                                        Title = "testNote1",
                                        Flags = 8,
                                        Content = "",
                                        FolderId = 1,
                                        Modified = DateTime.UtcNow
                                    },
                                    new NoteEntity
                                    {
                                        Title = "testNote2",
                                        Flags = 8,
                                        Content = "",
                                        FolderId = 1,
                                        Modified = DateTime.UtcNow
                                    }
                                );

                        context.SaveChanges();
                    }
                });
                _databaseInitialized = true;
            }
        }
        */

        builder.UseEnvironment("Integration Test");
    }
}
