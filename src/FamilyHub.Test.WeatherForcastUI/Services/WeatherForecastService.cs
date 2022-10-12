using FamilyHub.Test.WeatherForcastUI.Models;
using System.Text.Json;

namespace FamilyHub.Test.WeatherForcastUI.Services;

public interface IWeatherForecastService
{
    Task<List<WeatherForecast>> GetWeatherForecast();
}
public class WeatherForecastService: ApiService, IWeatherForecastService
{
    private readonly ITokenService _tokenService;
    public WeatherForecastService(HttpClient client, ITokenService tokenService) : base(client)
    {
        _tokenService = tokenService;
    }
    public async Task<List<WeatherForecast>> GetWeatherForecast()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "WeatherForecast"),
        };

        

        //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{_tokenService.GetToken()}");
        //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);


        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<WeatherForecast>();

    }
}
