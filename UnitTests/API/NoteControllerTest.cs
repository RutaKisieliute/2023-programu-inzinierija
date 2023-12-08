using Xunit;
using Moq;
using AALKisAPI.Controllers;
using AALKisAPI.Services;
using Microsoft.Extensions.Logging;
using AALKisShared.Records;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests.API;
public class NoteControllerTest
{
    private readonly NoteController _noteController;
    private readonly Mock<INotesRepository> _noteRepoMock = new Mock<INotesRepository>();
    private readonly ILogger<NoteController> _loggerMock = Mock.Of<ILogger<NoteController>>();

    public NoteControllerTest()
    {
        _noteController = new NoteController(_loggerMock, _noteRepoMock.Object);

    }

    [Fact]
    public async Task Get_ShouldReturnNote()
    {
        var noteId = 42;
        var noteTitle = "Test Title";
        var noteDto = new Note
        {
            Id = noteId,
            Title = noteTitle,
        };
        _noteRepoMock.Setup(x => x.GetNote(noteId, false)).Returns(noteDto);

        Note? note = await _noteController.Get(noteId);
    
        Assert.Equal(noteId, note.Value.Id);
    }
}
