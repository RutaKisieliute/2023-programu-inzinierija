using AALKisAPI.Data;
using FolderEntity = AALKisAPI.Models.Folder;
using NoteEntity = AALKisAPI.Models.Note;

namespace UnitTests.API;

public class TestNoteDBFixture
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    private static ConnectionString connectionString = new ConnectionString{
            Value = File.ReadAllText("./../../../AALKisAPI/Services/testdatabaselogin.txt")};

    public TestNoteDBFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.Folders.AddRange(
                                new FolderEntity{
                                    Title = "testFolder1",
                                    UserId = 1},
                                new FolderEntity{
                                    Title = "testFolder2",
                                    UserId = 1}
                            );
                    context.Notes.AddRange(
                                new NoteEntity{
                                    Title = "testNote1",
                                    Flags = 8,
                                    Content = "",
                                    FolderId = 1,
                                    Modified = DateTime.UtcNow
                                },
                                new NoteEntity{
                                    Title = "testNote2",
                                    Flags = 8,
                                    Content = "",
                                    FolderId = 1,
                                    Modified = DateTime.UtcNow
                                }
                            );

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public NoteDB CreateContext()
        => new NoteDB(connectionString);
}
