//using Xunit;
//using AALKisAPI.Services;
//using Note = AALKisShared.Records.Note;

//namespace UnitTests.API;

//public class UnitTestEFNotesRepository : IClassFixture<TestNoteDBFixture>
//{
//    public TestNoteDBFixture NoteDb {get;}

//    public UnitTestEFNotesRepository(TestNoteDBFixture noteDb)
//    {
//        NoteDb = noteDb;
//    }

//    [Theory]
//    [InlineData(1, "testNote1")]
//    [InlineData(2, "testNote2")]
//    public void GetNote_CorrectNote(int noteId, string expectedTitle)
//    {
//        using var context = NoteDb.CreateContext();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        var note = notesRepository.GetNote(noteId, true);

//        Assert.Equal(expectedTitle, note.Title);
//    }

//    [Theory]
//    [InlineData(1, true)]
//    [InlineData(2, true)]
//    [InlineData(4, false)]
//    public void CheckIfNoteExists_CorrectBool(int noteId, bool expectedResult)
//    {
//        using var context = NoteDb.CreateContext();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        var result = notesRepository.CheckIfNoteExists(noteId);

//        Assert.Equal(expectedResult, result);
//    }

//    [Theory]
//    [InlineData(1, "foobar", 3)]
//    [InlineData(1, "barfoo", 4)]
//    [InlineData(3, "barfoo", null)]
//    public void CreateNote_CorrectNoteId(int folderId, string noteTitle, int? expectedResult)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        int? result = null;
//        try
//        {
//            result = notesRepository.CreateNote(folderId, noteTitle, "");
//        }
//        catch (Exception)
//        {
//            Assert.Null(expectedResult);
//        }
//        finally
//        {
//            context.ChangeTracker.Clear();
//            Assert.Equal(expectedResult, result);
//        }
//    }

//    [Theory]
//    [InlineData(1)]
//    [InlineData(2)]
//    public void DeleteNote_NullNoteById(int noteId)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        notesRepository.DeleteNote(noteId);

//        context.ChangeTracker.Clear();

//        Assert.Null(context.Notes.FirstOrDefault(x => x.Id == noteId));
//    }

//    [Theory]
//    [InlineData(1, "foobar")]
//    [InlineData(2, "barfoo")]
//    public void UpdateNote_UpdatedContent(int noteId, string newContent)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        Note record = notesRepository.GetNote(noteId, false);
//        record.Update(content: newContent);

//        notesRepository.UpdateNote(record);

//        context.ChangeTracker.Clear();

//        Assert.Equal(newContent, context.Notes.First(x => x.Id == noteId).Content);
//    }

//    [Theory]
//    [InlineData("testNote", 2)]
//    [InlineData("Note1", 1)]
//    [InlineData("Note2", 1)]
//    public void SearchNotes_CorrectCount(string searchQuery, int expectedCount)
//    {
//        using var context = NoteDb.CreateContext();
//        context.Database.BeginTransaction();
//        var notesRepository = new EFNotesRepository(context, new EFTagsRepository(context), new EFKeywordsRepository(context));

//        List<Note> records = notesRepository.SearchNotes(searchQuery).ToList();

//        context.ChangeTracker.Clear();

//        Assert.Equal(expectedCount, records.Count());
//    }
//}


