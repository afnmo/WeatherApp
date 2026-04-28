using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using WeatherApp.Models;
using WeatherApp.Helpers;

namespace WeatherApp.Controllers;

[Authorize]
public class WeatherController : Controller
{
    private readonly WeatherService _weatherService;
    private readonly WeatherDbService _weatherDb;
    private readonly UserManager<ApplicationUser> _userManager;

    public WeatherController(
        WeatherService weatherService,
        WeatherDbService weatherDb,
        UserManager<ApplicationUser> userManager)
    {
        _weatherService = weatherService;
        _weatherDb = weatherDb;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.Cities = CityList.Cities;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            ViewBag.Error = "City is required";
            return View();
        }

        var result = await _weatherService.GetWeatherAsync(city);

        if (result == null)
        {
            ViewBag.Error = "Failed to fetch weather data";
            return View();
        }

        ViewBag.City = city;
        ViewBag.Cities = CityList.Cities;
        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save(string city, decimal humidity, decimal tempMin, decimal tempMax)
    {
        var user = await _userManager.GetUserAsync(User);

        var record = new WeatherRecord
        {
            City = city,
            Humidity = humidity,
            TempMin = tempMin,
            TempMax = tempMax,
            UserId = user!.Id
        };

        await _weatherDb.SaveWeatherAsync(record);

        return RedirectToAction("History");
    }

    [HttpGet]
    public async Task<IActionResult> History(string? city)
    {
        var user = await _userManager.GetUserAsync(User);
        var records = await _weatherDb.GetWeatherHistoryAsync(user!.Id, city);
        ViewBag.CityFilter = city;
        ViewBag.Cities = CityList.Cities;
        return View(records);
    }

    [HttpPost]
    public async Task<IActionResult> Update(List<WeatherRecord> records)
    {
        var user = await _userManager.GetUserAsync(User);

        foreach (var updated in records)
        {
            var original = await _weatherDb.GetWeatherRecordByIdAsync(updated.Id);

            if (original == null) continue;
            
            if (original.Humidity != updated.Humidity)
            {
                await _weatherDb.SaveAuditAsync(new WeatherAudit
                {
                    WeatherRecordId = updated.Id,
                    FieldName = "Humidity",
                    OldValue = original.Humidity.ToString(),
                    NewValue = updated.Humidity.ToString(),
                    ChangedByUserId = user!.Id,
                    ChangedAt = DateTime.UtcNow
                });
            }

            if (original.TempMin != updated.TempMin)
            {
                await _weatherDb.SaveAuditAsync(new WeatherAudit
                {
                    WeatherRecordId = updated.Id,
                    FieldName = "TempMin",
                    OldValue = original.TempMin.ToString(),
                    NewValue = updated.TempMin.ToString(),
                    ChangedByUserId = user!.Id,
                    ChangedAt = DateTime.UtcNow
                });
            }

            if (original.TempMax != updated.TempMax)
            {
                await _weatherDb.SaveAuditAsync(new WeatherAudit
                {
                    WeatherRecordId = updated.Id,
                    FieldName = "TempMax",
                    OldValue = original.TempMax.ToString(),
                    NewValue = updated.TempMax.ToString(),
                    ChangedByUserId = user!.Id,
                    ChangedAt = DateTime.UtcNow
                });
            }
            
            await _weatherDb.UpdateWeatherAsync(updated);
        }

        return RedirectToAction("History");
    }
}