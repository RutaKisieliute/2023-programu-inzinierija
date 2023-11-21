using AALKisMVCUI.Utility;

using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;
public class AppFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        /*
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(new APIClient(this.CreateClient()));
        });
        */
    }
}
