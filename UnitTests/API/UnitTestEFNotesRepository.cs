using Xunit;

namespace UnitTests.API;

public class UnitTestEFNotesRepository : IClassFixture<TestNoteDBFixture>
{
    public TestNoteDBFixture NoteDb {get;}
    public UnitTestEFNotesRepository(TestNoteDBFixture noteDb)
    {
        NoteDb = noteDb;
    }
}


