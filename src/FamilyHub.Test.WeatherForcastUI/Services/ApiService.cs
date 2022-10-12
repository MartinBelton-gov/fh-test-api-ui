using FamilyHub.Test.WeatherForcastUI.Models;
using System.Text.Json;

namespace FamilyHub.Test.WeatherForcastUI.Services;

public interface IApiService
{
}
public class ApiService : IApiService
{
    protected readonly HttpClient _client;

    public ApiService(HttpClient client)
    {
        _client = client;
    }

}
