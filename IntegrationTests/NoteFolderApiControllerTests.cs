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
        _folderId = null;
        _noteId = null;
    }

    // Start: Delete note id = 0, folder id = 0
    // Folder: Create, Exists, GetAll, GetOne, Rename
    // Note: Create, Exists, GetAll, GetOne, Rename, Delete
    // Folder: Delete
    int? _folderId;
    int? _noteId;

    [Fact]
    public async Task Post_CreateFolder_ReturnsOkResult()
    {
        // Arrange

        // Act
        var folderId = await _client.Fetch<int?>("/Folder/TestFolderName", HttpMethod.Post);
        _folderId = folderId;
        // Assert
        Assert.NotNull(folderId);
    }

    [Fact]
    public async Task Head_FolderExists_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Folder/{_folderId}", HttpMethod.Head);

        // Assert
        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task Get_GetAllFolders_ReturnsOkResult()
    {
        // Arrange

        // Act
        var folders = await _client.Fetch<IEnumerable<Folder>>("/Folder", HttpMethod.Get);

        // Assert
        Assert.NotEmpty(folders);
    }

    [Fact]
    public async Task Get_GetOneFolder_ReturnsOkResult()
    {
        // Arrange

        // Act
        var folder = await _client.Fetch<Folder>($"/Folder/{_folderId}", HttpMethod.Get);

        // Assert
        Assert.NotNull(folder);
    }

    [Fact]
    public async Task Patch_RenameFolder_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Folder/{_folderId}/TestFolderNameRenamed", HttpMethod.Patch);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_CreateNote_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Note/Create/{_folderId}/TestNoteName", HttpMethod.Post);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Head_NoteExists_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Note/{_noteId}", HttpMethod.Head);

        // Assert
        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task Get_GetOneNote_ReturnsOkResult()
    {
        // Arrange

        // Act
        var folder = await _client.Fetch<Folder>($"/Note/{_noteId}", HttpMethod.Get);

        // Assert
        Assert.NotNull(folder);
    }

    [Fact]
    public async Task Patch_RenameNote_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Note/{_noteId}/TestNoteRenamed", HttpMethod.Patch);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Delete_DeleteNote_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Note/{_noteId}", HttpMethod.Delete);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Delete_DeleteFolder_ReturnsOkResult()
    {
        // Arrange

        // Act
        var response = await _client.Fetch($"/Fodler/{_folderId}", HttpMethod.Delete);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
