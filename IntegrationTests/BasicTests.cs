using Xunit;

namespace IntegrationTests;

// Works only when API Server is running locally.
public class BasicTests : IClassFixture<AppFactory<AALKisMVCUI.Program>>
{
    private readonly AppFactory<AALKisMVCUI.Program> _factory;
    public BasicTests(AppFactory<AALKisMVCUI.Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Privacy")]
    [InlineData("/MyNotes")]
    [InlineData("/Archived")]
    [InlineData("/Editor/1")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }
}
