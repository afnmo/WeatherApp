using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        try
        {
            var apiKey = _config["OpenWeather:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("API key is missing");

            // current weather for humidity
            var currentUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
            var currentResponse = await _httpClient.GetFromJsonAsync<JsonElement>(currentUrl);

            if (!currentResponse.TryGetProperty("main", out var main))
                return null;

            var humidity = main.GetProperty("humidity").GetDecimal();

            // forecast for real min/max of today
            var forecastUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric&cnt=8";
            var forecastResponse = await _httpClient.GetFromJsonAsync<JsonElement>(forecastUrl);

            if (!forecastResponse.TryGetProperty("list", out var list))
                return null;

            var temps = new List<decimal>();

            foreach (var item in list.EnumerateArray())
            {
                if (item.TryGetProperty("main", out var forecastMain))
                {
                    temps.Add(forecastMain.GetProperty("temp_min").GetDecimal());
                    temps.Add(forecastMain.GetProperty("temp_max").GetDecimal());
                }
            }

            if (!temps.Any())
                return null;

            return new WeatherResult
            {
                Humidity = humidity,
                TempMin = temps.Min(),
                TempMax = temps.Max()
            };
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}