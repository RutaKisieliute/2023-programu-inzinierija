using Xunit;
using AALKisAPI.Services;
using Note = AALKisShared.Records.Note;

namespace UnitTests.API;

public class UnitTestEFNotesRepository : IClassFixture<TestNoteDBFixture>
{
    public TestNoteDBFixture NoteDb {get;}
    public UnitTestEFNotesRepository(TestNoteDBFixture noteDb)
    {
        NoteDb = noteDb;
    }

    [Theory]
    [InlineData(1, "testNote1")]
    [InlineData(2, "testNote2")]
    public void GetNote_CorrectNote(int noteId, string expectedTitle)
    {
        using var context = NoteDb.CreateContext();
        var notesRepository = new EFNotesRepository(context);

        var note = notesRepository.GetNote(noteId, true);

        Assert.Equal(expectedTitle, note.Title);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(4, false)]
    public void CheckIfNoteExists_CorrectBool(int noteId, bool expectedResult)
    {
        using var context = NoteDb.CreateContext();
        var notesRepository = new EFNotesRepository(context);

        var result = notesRepository.CheckIfNoteExists(noteId);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(1, "foobar", 3)]
    [InlineData(1, "barfoo", 3)]
    [InlineData(3, "barfoo", null)]
    public void CreateNote_CorrectNoteId(int folderId, string noteTitle, int? expectedResult)
    {
        using var context = NoteDb.CreateContext();
        context.Database.BeginTransaction();
        var notesRepository = new EFNotesRepository(context);

        var result = notesRepository.CreateNote(folderId, noteTitle);

        context.ChangeTracker.Clear();

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void DeleteNote_NullNoteById(int noteId)
    {
        using var context = NoteDb.CreateContext();
        context.Database.BeginTransaction();
        var notesRepository = new EFNotesRepository(context);

        notesRepository.DeleteNote(noteId);

        context.ChangeTracker.Clear();

        Assert.Null(context.Notes.First(x => x.Id == noteId));
    }

    [Theory]
    [InlineData(1, "foobar")]
    [InlineData(2, "barfoo")]
    public void UpdateNote_UpdatedContent(int noteId, string newContent)
    {
        using var context = NoteDb.CreateContext();
        context.Database.BeginTransaction();
        var notesRepository = new EFNotesRepository(context);

        Note record = notesRepository.GetNote(noteId, false);
        record.Update(content: newContent);

        notesRepository.UpdateNote(record);

        context.ChangeTracker.Clear();

        Assert.Equal(newContent, context.Notes.First(x => x.Id == noteId).Content);
    }

    [Theory]
    [InlineData("testNote", 2)]
    [InlineData("Note1", 1)]
    [InlineData("Note2", 1)]
    public void SearchByTitle_CorrectCount(string searchQuery, int expectedCount)
    {
        using var context = NoteDb.CreateContext();
        context.Database.BeginTransaction();
        var notesRepository = new EFNotesRepository(context);

        List<Note> records = notesRepository.SearchByTitle(searchQuery);

        context.ChangeTracker.Clear();

        Assert.Equal(expectedCount, records.Count());
    }
}


