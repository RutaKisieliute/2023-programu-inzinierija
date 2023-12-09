using Xunit;
using AALKisMVCUI.Controllers;
namespace UnitTests.MVCUI.Controllers;

public class UnitTestSearchController
{
    [Theory]
    [InlineData("Accepted theories can provide satisfactory results, and thus experiments can be avoided.", "thou", "<a>", "</a>")]
    [InlineData("Accepted theories can provide satisfactory results, and thus experiments can be avoided.", "sun-tzu, art of war", "<a>", "</a>")]
    public void InsertStringsBetweenAllSubstrings_NoSubstring_Unchanged(string original, string substring, string before, string after)
    {
        
        string result = SearchController.InsertStringsBetweenAllSubstrings(original, substring, before, after);

        Assert.Equal(original, result);
    }

    [Fact]
    public void InsertStringsBetweenAllSubstrings_MultipleSubstrings_WrappedInTags()
    {
        string original = "Accepted theories can provide satisfactory results, and thus experiments can be avoided. Theories. Theory can only take you so far.";
        string substring = "Theories";
        string before = "<span>";
        string after = "</span>";

        string result = SearchController.InsertStringsBetweenAllSubstrings(original, substring, before, after);

        Assert.Equal(result, "Accepted <span>theories</span> can provide satisfactory results, and thus experiments can be avoided. <span>Theories</span>. Theory can only take you so far.");
    }

}
