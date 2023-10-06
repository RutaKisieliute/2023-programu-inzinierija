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

    public async Task<string?> Fetch(string uri, HttpMethod method, string? content = null)
    {
        HttpRequestMessage request = new HttpRequestMessage(method, uri);
        if(content != null)
        {
            request.Content = new StringContent(content);
        }
        HttpResponseMessage response = Client.Send(request);
        if(!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException($"Failed to execute fetch"
                    + $" to {uri} with {method} {content}",
                    (int)response.StatusCode);
        }
        if(response == null || response.Content.Headers.ContentLength == 0)
        {
            return null;
        }
        return await response.Content.ReadAsStringAsync();
    }
}
