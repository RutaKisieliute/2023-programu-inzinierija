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

    public async Task<string> GetContents(string requestUri)
    {
        HttpResponseMessage response = await Client.GetAsync(requestUri);

        if(!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException($"API server is running,"
                    + " but failed to get {requestUri}",
                    (int)response.StatusCode);
        }

        string contents = await response.Content.ReadAsStringAsync();

        return contents;
    }

    public async void PostContents(string requestUri, HttpContent content)
    {
        HttpResponseMessage response = await Client.PostAsync(requestUri, content);

        if(!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException($"API server is running,"
                    + " but failed to post {requestUri}",
                    (int)response.StatusCode);
        }

        return;
    }
}
