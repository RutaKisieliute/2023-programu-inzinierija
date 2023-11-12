using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using AALKisMVCUI;
using AALKisAPI;
using MySqlX.XDevAPI;
using AALKisMVCUI.Utility;

namespace IntegrationTests;

public class MyNotesControllerTests : IClassFixture<WebApplicationFactory<AALKisMVCUI.Program>>
{
    private readonly WebApplicationFactory<AALKisMVCUI.Program> _factory;

    public MyNotesControllerTests(WebApplicationFactory<AALKisMVCUI.Program> factory)
    {
        //var webAppFactory = new WebApplicationFactory<AALKisAPI.Program>();
        //var httpClient = webAppFactory.CreateClient();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                
            });
        });
    }

    [Theory]
    [InlineData("/MyNotes")] // FAIL: Because does API call. No idea, how to launch and pass API client to MVC client in a test.
    [InlineData("/")] // PASS
    [InlineData("/Home/Privacy")] // PASS
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
