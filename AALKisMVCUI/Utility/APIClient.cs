using Newtonsoft.Json;

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

    public async Task<HttpResponseMessage> Fetch(string uri, HttpMethod method, HttpContent? content = null)
    {

        Console.WriteLine($"URI: {uri}");
        Console.WriteLine($"Method: {method}");
        if (content != null)
        {
            Console.WriteLine($"Content: {await content.ReadAsStringAsync()}");
        }
        else
        {
            Console.WriteLine("Content is null");
        }


        HttpRequestMessage request = new HttpRequestMessage(method, uri) { Content = content };
        HttpResponseMessage response = await Client.SendAsync(request);

        Console.WriteLine($"Response status code: {response.StatusCode}");
        Console.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response throw status code: {response.StatusCode}");
            throw new BadHttpRequestException($"Failed to execute fetch"
                    + $" to {uri} with {method} {content}",
                    (int)response.StatusCode);
        }
        Console.WriteLine($"Response send status code: {response.StatusCode}");
        return response;
        //if(response == null || response.Content.Headers.ContentLength == 0)
        //{
        //    return null;
        //}
        //return await response.Content.ReadAsStringAsync();
    }

    public async Task<T?> Fetch<T>(string uri, HttpMethod method, HttpContent? content = null)
    {
        var response = await Fetch(uri, method, content);
        string json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(json);
    }

    //public async Task<T?> Fetch<Http>(string uri, HttpMethod method, HttpContent? content = null)
    //{
    //    return default(T);
    //}
}
