using System.Text;
using AALKisAPI.Models;
using AALKisShared.Utility;
using AALKisMVCUI.Utility;
using Xunit;

namespace IntegrationTests.API;

// Works only when API Server is running locally.
public class NoteControllerTests : IClassFixture<AppFactory<AALKisAPI.Program>>
{
    private readonly AppFactory<AALKisAPI.Program> _factory;
    private readonly APIClient _client;
    public NoteControllerTests(AppFactory<AALKisAPI.Program> factory)
    {
        _factory = factory;
        _client = new APIClient(_factory.CreateClient());
    }

    [Fact]
    public async Task Post_CreateNote_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Note/Create/{1}/TestNoteName", HttpMethod.Post);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Head_NoteExists_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Note/{1}", HttpMethod.Head);

        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task Get_GetOneNote_ReturnsOkResult()
    {
        var folder = await _client.Fetch<Folder>($"/Note/{1}", HttpMethod.Get);

        Assert.NotNull(folder);
    }

    [Fact]
    public async Task Put_RenameNote_ReturnsOkResult()
    {
        AALKisShared.Records.Note fieldsToUpdate = new AALKisShared.Records.Note();
        fieldsToUpdate.Title = "Updated Title";
        string jsonString = fieldsToUpdate.ToJsonString();

        var response = await _client.Fetch($"Note/{1}",
                    HttpMethod.Put,
                    new StringContent(jsonString, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Delete_DeleteNote_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Note/{2}", HttpMethod.Delete);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_SearchNotes_ReturnsOkResult()
    {
        var response = await _client.Fetch($"/Note/Search/hello", HttpMethod.Get);

        response.EnsureSuccessStatusCode();
    }
}
