using Xunit;

namespace IntegrationTests;

// Works only when API Server is running locally.
public class MyNotesControllerTests : IClassFixture<AppFactory<AALKisMVCUI.Program>>
{
    private readonly AppFactory<AALKisMVCUI.Program> _factory;
    public MyNotesControllerTests(AppFactory<AALKisMVCUI.Program> factory)
    {
        _factory = factory;
    }
    /*
    [Fact]
    public async Task Post_CreateEmptyFolder_ReturnsOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/MyNotes/CreateEmptyFolder/TestFolderName", null);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_CreateEmptyNote_ReturnsOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/MyNotes/CreateEmptyNote/0", null);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    */
}
