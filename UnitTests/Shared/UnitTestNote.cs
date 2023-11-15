using Xunit;
using AALKisShared;
using AALKisShared.Enums;

namespace UnitTests;

public class UnitTestNote
{
    [Fact]
    public void IsValid_InvalidNotes_ReturnsFalse()
    {
        Assert.False(new Note().IsValid());
        Assert.False(new Note{Id = 1}.IsValid());
        Assert.False(new Note{Content = "foo"}.IsValid());
        Assert.False(new Note{Title = "foo", Content = "bar"}.IsValid());
    }

    [Fact]
    public void IsValid_InvalidTitle_ReturnsFalse()
    {
        Assert.False(new Note{Id = 1,
                    Title = "foo#",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "due to all known laws of aviation.",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "\t",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "#",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "\x5C",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "/",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "<li></li>",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.False(new Note{Id = 1,
                    Title = "?root=true",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
    }

    [Fact]
    public void IsValid_ValidNotes_ReturnsTrue()
    {
        Assert.True(new Note{Id = 1,
                    Title = "due to all known laws of aviation,",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.True(new Note{Id = 1,
                    Title = "foo",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.True(new Note{Id = 1,
                    Title = "you can use spaces in the title",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
         Assert.True(new Note{Id = 1,
                    Title = "1234-05-06 07:08:09",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());
        Assert.True(new Note{Id = 1,
                    Title = "@Friendo",
                    Content = "bar",
                    EditDate = DateTime.Now,
                    Flags = 0}
                .IsValid());

    }

    [Fact]
    public void Update_SinglePropertyUpdates_AllEqual()
    {
        Note testNote = new Note{};
        Assert.Equal(testNote.Id, -1);

        var newTitle = "PROUD TO BE ALBANIAN";
        testNote.Update(title: newTitle);
        Assert.Equal(testNote.Title, newTitle);

        var newContent = "FOR MY FLAG I DIE";
        testNote.Update(content: newContent);
        Assert.Equal(testNote.Content, newContent);

        var newEditDate = DateTime.Now;
        testNote.Update(editDate: newEditDate);
        Assert.Equal(testNote.EditDate, newEditDate);

        var newFlags = NoteFlags.None;
        testNote.Update(flags: newFlags);
        Assert.Equal(testNote.Flags, newFlags);
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
