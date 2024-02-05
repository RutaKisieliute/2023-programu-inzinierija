using Xunit;

namespace UnitTests.API;

public class UnitTestEFKeywordsRepository : IClassFixture<TestNoteDBFixture>
{
    public TestNoteDBFixture NoteDb {get;}
    public UnitTestEFKeywordsRepository(TestNoteDBFixture noteDb)
    {
        NoteDb = noteDb;
    }
}

