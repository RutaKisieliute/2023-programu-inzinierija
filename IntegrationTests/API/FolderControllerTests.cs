using System.Text;
using AALKisAPI.Models;
using AALKisShared.Utility;
using AALKisMVCUI.Utility;
using Xunit;

namespace IntegrationTests.API;

// Works only when API Server is running locally.
public class FolderControllerTests : IClassFixture<AppFactory<AALKisAPI.Program>>
{
    private readonly AppFactory<AALKisAPI.Program> _factory;
    private readonly APIClient _client;
    public FolderControllerTests(AppFactory<AALKisAPI.Program> factory)
    {
        _factory = factory;
        _client = new APIClient(_factory.CreateClient());
    }

    [Fact]
    public async Task Post_CreateFolder_ReturnsOkResult()
    {
        var folderId = await _client.Fetch<int?>("/Folder/TestFolderName", HttpMethod.Post);

        Assert.NotNull(folderId);
    }

    [Fact]
    public async Task Head_FolderExists_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Folder/{1}", HttpMethod.Head);

        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task Get_GetAllFolders_ReturnsOkResult()
    {
        var folders = await _client.Fetch<IEnumerable<Folder>>("/Folder", HttpMethod.Get);

        Assert.NotEmpty(folders);
    }

    [Fact]
    public async Task Get_GetOneFolder_ReturnsOkResult()
    {
        var folder = await _client.Fetch<Folder>($"/Folder/{1}", HttpMethod.Get);

        Assert.NotNull(folder);
    }

    [Fact]
    public async Task Patch_RenameFolder_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Folder/{1}/TestFolderNameRenamed", HttpMethod.Patch);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Delete_DeleteFolder_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Folder/{2}", HttpMethod.Delete);

        response.EnsureSuccessStatusCode();
    }
}
