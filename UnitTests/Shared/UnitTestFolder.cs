using Xunit;
using AALKisShared.Records;

namespace UnitTests.Shared;

public class UnitTestFolder
{
    [Fact]
    public void CompareTo_NameLength_ShorterMoreThanLonger()
    {
        Folder<int> testFolder = new Folder<int>{Name = "short name"};
        Folder<int> otherTestFolder = new Folder<int>{Name = "loooong name"};
        Assert.True(testFolder.CompareTo(otherTestFolder) > 0);
    }
}
