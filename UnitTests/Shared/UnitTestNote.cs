using Xunit;
using AALKisShared.Records;
using AALKisShared.Enums;

namespace UnitTests.Shared;

public class UnitTestNote
{
    [Theory]
    [InlineData()]
    [InlineData(1)]
    [InlineData(null, null, "foo")]
    [InlineData(null, "foo", "bar")]
    public void IsValid_InvalidNotes_ReturnsFalse(int? id = null, string? title = null, string? content = null)
    {
        var note = new Note{Title = title,
            Content = content,
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};
        note.Id = id ?? note.Id;

        Assert.False(note.IsValid());
    }

    [Theory]
    [InlineData("foo#")]
    [InlineData("due to all known laws of aviation.")]
    [InlineData("")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("#")]
    [InlineData("\x5C")]
    [InlineData("/")]
    [InlineData("<li></li>")]
    [InlineData("?root=true")]
    public void IsValid_InvalidTitle_ReturnsFalse(string title)
    {
        var note = new Note{Id = 1,
            Title = title,
            Content = "bar",
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};

        Assert.False(note.IsValid());
    }

    [Theory]
    [InlineData("due to all known laws of aviation,")]
    [InlineData("foo")]
    [InlineData("you can use spaces in the title")]
    [InlineData("1234-05-06 07:08:09")]
    [InlineData("@Friendo")]
    public void IsValid_ValidNotes_ReturnsTrue(string title)
    {
        var note = new Note{Id = 1,
                    Title = title,
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = NoteFlags.None};

        Assert.True(note.IsValid());
    }

    [Theory]
    [InlineData("Red n'black I dress eagle on my chest")]
    [InlineData("It's good to be an ALBANIAN")]
    [InlineData("Keep my head up high for that flag I die")]
    [InlineData("I'm proud to be an ALBANIAN")]
    public void Update_Title_Equal(string title)
    {
        var note = new Note{Id = 1,
            Title = "foo",
            Content = "bar",
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};

        note.Update(title: title);

        Assert.Equal(note.Title, title);
    }

    [Theory]
    [InlineData("Red n'black I dress eagle on my chest")]
    [InlineData("It's good to be an ALBANIAN")]
    [InlineData("Keep my head up high for that flag I die")]
    [InlineData("I'm proud to be an ALBANIAN")]
    public void Update_Content_Equal(string content)
    {
        var note = new Note{Id = 1,
            Title = "foo",
            Content = "bar",
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};

        note.Update(content: content);

        Assert.Equal(note.Content, content);
    }

    [Theory]
    [InlineData(1987, 01, 01)]
    public void Update_EditTime_Equal(int year, int month, int day)
    {
        var note = new Note{Id = 1,
            Title = "foo",
            Content = "bar",
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};
        var date = new DateTime(year, month, day);

        note.Update(editDate: date);

        Assert.Equal(note.EditDate, date);
    }

    [Theory]
    [InlineData(NoteFlags.Public)]
    [InlineData(NoteFlags.Archived)]
    public void Update_Flags_Equal(NoteFlags flags)
    {
        var note = new Note{Id = 1,
            Title = "foo",
            Content = "bar",
            EditDate = DateTime.Now,
            Flags = NoteFlags.None};

        note.Update(flags: flags);

        Assert.Equal(note.Flags, flags);
    }

    [Fact]
    public void CompareTo_ArchivedVersusNormal_ArchivedMoreThanNormal()
    {
        Note archivedNote = new Note{Flags = NoteFlags.Archived, Title = "short title"};
        Note normalNote = new Note{Flags = NoteFlags.None, Title = "short title"};

        Assert.True(archivedNote.CompareTo(normalNote) > 0);
        Assert.True(normalNote.CompareTo(archivedNote) < 0);

    }

    [Fact]
    public void CompareTo_TitleLength_ShorterMoreThanLonger()
    {
        Note archivedNote = new Note{Flags = NoteFlags.Archived, Title = "short title"};
        Note normalNote = new Note{Flags = NoteFlags.None, Title = "short title"};
        Note otherArchivedNote = new Note{Flags = NoteFlags.Archived, Title = "loooong title"};
        Note otherNormalNote = new Note{Flags = NoteFlags.None, Title = "loooong title"};

        Assert.True(archivedNote.CompareTo(otherArchivedNote) > 0);
        Assert.True(normalNote.CompareTo(otherNormalNote) > 0);
    }
}
