using Xunit;
using AALKisMVCUI.Controllers;
using AALKisShared.Records;
using AALKisShared.Utility;

namespace UnitTests.MVCUI.Controllers;

public class UnitTestEditorController
{
    [Theory]
    [InlineData(1, "#foo")]
    [InlineData(-1, "/foo")]
    [InlineData(-2, "\\foo")]
    public void CreateValidatedNote_InvalidNote_ThrowException(int id, string title)
    {
        Note note = new Note{Id = id, Title = title};
        string json = note.ToJsonString();

        try
        {
            Note validatedNote = EditorController.CreateValidatedNote(json);
        }
        catch { return; }

        Assert.Fail("Must throw an exception");
    }

    [Theory]
    [InlineData("<li></li>", "&lt;li&gt;&lt;/li&gt;")]
    [InlineData("\"amongus\"", "&quot;amongus&quot;")]
    [InlineData("'ahrombus'", "&#39;ahrombus&#39;")]
    public void CreateValidatedNote_Contents_SanitizeHTML(string content, string expected)
    {
        Note note = new Note{Id = 1, Title = "title", Content = content};
        string json = note.ToJsonString();

        Note validatedNote = EditorController.CreateValidatedNote(json);

        Assert.Equal(validatedNote.Content, expected);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData("Vroom vroom", 0)]
    [InlineData("$Vroom vroom", 1)]
    [InlineData("$Vroom $vroom", 1)]
    [InlineData("$Vroom $froom", 2)]
    public void GetKeywordsFromString_CorrectCount(string content, int expectedCount)
    {
        var hashset = EditorController.GetKeywordsFromString(content);

        Assert.Equal(expectedCount, hashset.Count());
    }
}
