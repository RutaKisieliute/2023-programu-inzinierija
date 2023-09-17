using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
namespace AALKisUI.Pages;

 

public class WeatherForecastModel : PageModel
{
    private readonly HttpClient _httpClient;

    public WeatherForecastModel(HttpClient httpClient)
    {
        _httpClient = httpClient;

        System.Net.ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;
    }

    public List<WeatherForecastData>? WeatherData { get; set; }

    public async Task OnGetAsync()
    {
        // Display "Loading..." while making the GET request
        // You can also show a loading spinner using JavaScript/CSS
        WeatherData = null;

        var response = await _httpClient.GetAsync("https://localhost:7014/WeatherForecast");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            WeatherData = JsonConvert.DeserializeObject<List<WeatherForecastData>>(json);
        }
    }
}

