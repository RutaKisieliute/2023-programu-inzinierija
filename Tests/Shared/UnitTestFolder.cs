using Xunit;
using AALKisShared;

namespace Tests;

public class UnitTestFolder
{
    [Fact]
    public void CompareTo()
    {
        Folder<int> testFolder = new Folder<int>{Name = "short name"};
        Folder<int> otherTestFolder = new Folder<int>{Name = "loooong name"};
        Assert.True(testFolder.CompareTo(otherTestFolder) > 0);
    }
}
