using System.Security.Policy;

using AALKisAPI.Models;

using AALKisMVCUI.Utility;

using Xunit;

namespace IntegrationTests;

// Works only when API Server is running locally.
public class NoteFolderApiControllerTests : IClassFixture<AppFactory<AALKisAPI.Program>>
{
    private readonly AppFactory<AALKisAPI.Program> _factory;
    private readonly APIClient _client;
    public NoteFolderApiControllerTests(AppFactory<AALKisAPI.Program> factory)
    {
        _factory = factory;
        _client = new APIClient(_factory.CreateClient());
    }

    // Start: Delete note id = 0, folder id = 0
    // Folder: Create, Exists, GetAll, GetOne, Rename
    // Note: Create, Exists, GetAll, GetOne, Rename, Delete
    // Folder: Delete

    [Fact]
    public async Task TestFolderAndNoteCreationPipeline()
    {
        // Arrange

        // Act
        var folderId = await _client.Fetch<int?>("/Folder/TestFolderName", HttpMethod.Post);
        // Assert
        Assert.NotNull(folderId);

        // Arrange

        // Act
        var response = await _client.Fetch($"/Folder/{folderId}", HttpMethod.Head);

        // Assert
        response.EnsureSuccessStatusCode();

        // Arrange

        // Act
        var folders = await _client.Fetch<IEnumerable<Folder>>("/Folder", HttpMethod.Get);

        // Assert
        Assert.NotNull(folders);
        Assert.NotEmpty(folders);


        // Arrange

        // Act
        var folder = await _client.Fetch<Folder>($"/Folder/{folderId}", HttpMethod.Get);

        // Assert
        Assert.NotNull(folder);

        // Arrange

        // Act
        var response2 = await _client.Fetch($"/Folder/{folderId}/TestFolderNameRenamed", HttpMethod.Patch);

        // Assert
        response2.EnsureSuccessStatusCode();

        // Arrange

        // Act
        var noteId = await _client.Fetch<int?>($"/Note/Create/{folderId}/TestNoteName", HttpMethod.Post);

        // Assert
        Assert.NotNull(noteId);

        // Arrange

        // Act
        var response3 = await _client.Fetch($"/Note/{noteId}", HttpMethod.Head);

        // Assert
        response.EnsureSuccessStatusCode();


        // Arrange

        // Act
        var folder2 = await _client.Fetch<Folder>($"/Note/{noteId}", HttpMethod.Get);

        // Assert
        Assert.NotNull(folder2);

        // Arrange
        // TODO ADD UPDATE
        // Act
        //var response4 = await _client.Fetch($"/Note/{noteId}", HttpMethod.Put, );

        // Assert
        //response4.EnsureSuccessStatusCode();

        // Arrange

        // Act
        var response5 = await _client.Fetch($"/Note/{noteId}", HttpMethod.Delete);

        // Assert
        response5.EnsureSuccessStatusCode();

        // Arrange

        // Act
        var response6 = await _client.Fetch($"/Folder/{folderId}", HttpMethod.Delete);

        // Assert
        response6.EnsureSuccessStatusCode();
    }

}
