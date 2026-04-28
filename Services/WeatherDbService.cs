using Microsoft.Data.SqlClient;
using System.Data;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherDbService
{
    private readonly IConfiguration _config;

    public WeatherDbService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SaveWeatherAsync(WeatherRecord record)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_InsertWeatherRecord", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@City", record.City);
        cmd.Parameters.AddWithValue("@Humidity", record.Humidity);
        cmd.Parameters.AddWithValue("@TempMin", record.TempMin);
        cmd.Parameters.AddWithValue("@TempMax", record.TempMax);
        cmd.Parameters.AddWithValue("@UserId", record.UserId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<WeatherRecord>> GetWeatherHistoryAsync(string userId, string? city = null)
    {
        var records = new List<WeatherRecord>();

        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_GetWeatherHistory", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@City", string.IsNullOrWhiteSpace(city) ? DBNull.Value : city);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            records.Add(new WeatherRecord
            {
                Id = (int)reader["Id"],
                City = reader["City"].ToString()!,
                Humidity = (decimal)reader["Humidity"],
                TempMin = (decimal)reader["TempMin"],
                TempMax = (decimal)reader["TempMax"],
                RecordDate = (DateTime)reader["RecordDate"]
            });
        }

        return records;
    }

    public async Task UpdateWeatherAsync(WeatherRecord record)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_UpdateWeatherRecord", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", record.Id);
        cmd.Parameters.AddWithValue("@Humidity", record.Humidity);
        cmd.Parameters.AddWithValue("@TempMin", record.TempMin);
        cmd.Parameters.AddWithValue("@TempMax", record.TempMax);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SaveAuditAsync(WeatherAudit audit)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_InsertAudit", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@WeatherRecordId", audit.WeatherRecordId);
        cmd.Parameters.AddWithValue("@FieldName", audit.FieldName);
        cmd.Parameters.AddWithValue("@OldValue", audit.OldValue ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@NewValue", audit.NewValue ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ChangedByUserId", audit.ChangedByUserId);
        cmd.Parameters.AddWithValue("@ChangedAt", audit.ChangedAt);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<WeatherRecord?> GetWeatherRecordByIdAsync(int id)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_GetWeatherRecordById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new WeatherRecord
            {
                Id = (int)reader["Id"],
                City = reader["City"].ToString()!,
                Humidity = (decimal)reader["Humidity"],
                TempMin = (decimal)reader["TempMin"],
                TempMax = (decimal)reader["TempMax"],
                RecordDate = (DateTime)reader["RecordDate"],
                UserId = reader["UserId"].ToString()!
            };
        }

        return null;
    }
}