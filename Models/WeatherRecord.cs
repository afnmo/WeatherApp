namespace WeatherApp.Models;

public class WeatherRecord
{
    public int Id { get; set; }

    public string City { get; set; } = string.Empty;

    public decimal Humidity { get; set; }

    public decimal TempMin { get; set; }

    public decimal TempMax { get; set; }

    public DateTime RecordDate { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}