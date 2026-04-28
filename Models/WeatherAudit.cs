namespace WeatherApp.Models;

public class WeatherAudit
{
    public int Id { get; set; }
    public int WeatherRecordId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public WeatherRecord WeatherRecord { get; set; } = null!;
    public ApplicationUser ChangedByUser { get; set; } = null!;
}