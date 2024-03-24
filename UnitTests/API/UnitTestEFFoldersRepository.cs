//using Xunit;
//using AALKisAPI.Services;
//using AALKisShared.Records;

//namespace UnitTests.API;

//public class UnitTestEFFolderRepository : IClassFixture<TestNoteDBFixture>
//{
//    public TestNoteDBFixture NoteDb {get;}

//    public UnitTestEFFolderRepository(TestNoteDBFixture noteDb)
//    {
//        NoteDb = noteDb;
//    }

//    [Fact]
//    public void GetAllFolders_TwoNamedFolders()
//    {
//        using var context = NoteDb.CreateContext();
//        var folderRepository = new EFFoldersRepository(context);

//        var folders = folderRepository.GetAllFolders(true);

//        Assert.Equal("testFolder1", folders[0].Name);
//        Assert.Equal("testFolder2", folders[1].Name);
//    }

//    [Fact]
//    public void GetAllFolders_TwoNamedAndFilledFolders()
//    {
//        using var context = NoteDb.CreateContext();
//        var folderRepository = new EFFoldersRepository(context);

//        var folders = folderRepository.GetAllFolders(false);

//        Assert.Equal("testFolder1", folders[0].Name);
//        Assert.Equal(2, folders[0].Records.Count);
//        Assert.Equal("testFolder2", folders[1].Name);
//        Assert.Equal(0, folders[1].Records.Count);
//    }

//    [Fact]
//    public void GetFolder_FirstFolder_TwoNotes()
//    {
//        using var context = NoteDb.CreateContext();

//        var folderRepository = new EFFoldersRepository(context);

//        var folder = folderRepository.GetFolder(1, false);

//        Assert.Equal("testNote1", folder.Records[0].Title);
//        Assert.Equal("testNote2", folder.Records[1].Title);
//    }

//    [Theory]
//    [InlineData(1, true)]
//    [InlineData(0, false)]
//    public void CheckIfFolderExists(int id, bool expectedResult)
//    {
//        using var context = NoteDb.CreateContext();
//        var folderRepository = new EFFoldersRepository(context);

//        var result = folderRepository.CheckIfFolderExists(id);

//        Assert.Equal(result, expectedResult);
//    }

//    [Theory]
//    [InlineData(3)]
//    public void CreateFolder_CorrectlyCreatedFolder(int expectedId)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var folderRepository = new EFFoldersRepository(context);

//        var id = folderRepository.CreateFolder("foo bar world");

//        context.ChangeTracker.Clear();

//        var folder = context.Folders.Single(folder => folder.Id == expectedId);
//        Assert.Equal("foo bar world", folder.Title);
//        folder = context.Folders.Single(folder => folder.Id == id);
//        Assert.Equal("foo bar world", folder.Title);
//    }

//    [Theory]
//    [InlineData(1, 0)]
//    [InlineData(2, 2)]
//    [InlineData(3, 2)]
//    public void DeleteFolder_RemainingNotesCorrect(int id, int expectedCount)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var folderRepository = new EFFoldersRepository(context);

//        folderRepository.DeleteFolder(id, true);

//        context.ChangeTracker.Clear();

//        Assert.Equal(expectedCount, context.Notes.Count());
//    }

//    [Theory]
//    [InlineData("foobar")]
//    [InlineData("barfoo")]
//    public void RenameFolder_CorrectlyRenamesFolder(string newName)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var folderRepository = new EFFoldersRepository(context);

//        folderRepository.RenameFolder(1, newName);

//        context.ChangeTracker.Clear();

//        Assert.Equal(newName, context.Folders.First(x => x.Id == 1).Title);

//    }
//}
