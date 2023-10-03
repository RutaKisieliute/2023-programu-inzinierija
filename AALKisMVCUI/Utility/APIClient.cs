namespace AALKisMVCUI.Utility;

public class APIClient
{
    private static readonly Uri APIUri = new Uri("https://localhost:7014");

    public HttpClient Client { get; }

    public APIClient(HttpClient client)
    {
        client.BaseAddress = APIUri;
        client.DefaultRequestHeaders.Clear();
        Client = client;
        return;
    }
}
