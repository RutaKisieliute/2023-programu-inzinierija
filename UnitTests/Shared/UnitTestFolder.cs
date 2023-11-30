using Xunit;
using AALKisShared.Records;

namespace UnitTests.Shared;

public class UnitTestFolder
{
    [Fact]
    public void CompareTo_NameLength_ShorterMoreThanLonger()
    {
        Folder<Note> testFolder = new Folder<Note>{Name = "short name"};
        Folder<Note> otherTestFolder = new Folder<Note>{Name = "loooong name"};
        Assert.True(testFolder.CompareTo(otherTestFolder) > 0);
    }
}
