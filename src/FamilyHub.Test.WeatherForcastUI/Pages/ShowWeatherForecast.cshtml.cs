using FamilyHub.Test.WeatherForcastUI.Models;
using FamilyHub.Test.WeatherForcastUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Test.WeatherForcastUI.Pages;

public class ShowWeatherForecastModel : PageModel
{
    private readonly IWeatherForecastService _weatherForecastService;

    public List<WeatherForecast> WeatherForecasts { get; set; } = default!;

    public ShowWeatherForecastModel(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }
    public async Task OnGet()
    {
        WeatherForecasts = await _weatherForecastService.GetWeatherForecast();
    }
}
