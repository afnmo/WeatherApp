-- 1. Insert weather record
CREATE PROCEDURE sp_InsertWeatherRecord
    @City NVARCHAR(100),
    @Humidity DECIMAL(10,2),
    @TempMin DECIMAL(10,2),
    @TempMax DECIMAL(10,2),
    @UserId NVARCHAR(450)
AS
BEGIN
INSERT INTO WeatherRecords (City, Humidity, TempMin, TempMax, RecordDate, UserId)
VALUES (@City, @Humidity, @TempMin, @TempMax, GETDATE(), @UserId)
END
GO

-- 2. Get weather history
CREATE PROCEDURE sp_GetWeatherHistory
    @UserId NVARCHAR(450),
    @City NVARCHAR(100) = NULL
AS
BEGIN
SELECT Id, City, Humidity, TempMin, TempMax, RecordDate
FROM WeatherRecords
WHERE UserId = @UserId
  AND (@City IS NULL OR City = @City)
ORDER BY RecordDate DESC
END
GO

-- 3. Get single record by ID
CREATE PROCEDURE sp_GetWeatherRecordById
    @Id INT
AS
BEGIN
SELECT Id, City, Humidity, TempMin, TempMax, RecordDate, UserId
FROM WeatherRecords
WHERE Id = @Id
END
GO

-- 4. Update weather record
CREATE PROCEDURE sp_UpdateWeatherRecord
    @Id INT,
    @Humidity DECIMAL(10,2),
    @TempMin DECIMAL(10,2),
    @TempMax DECIMAL(10,2)
AS
BEGIN
UPDATE WeatherRecords
SET Humidity = @Humidity,
    TempMin = @TempMin,
    TempMax = @TempMax
WHERE Id = @Id
END
GO

-- 5. Insert audit log
CREATE PROCEDURE sp_InsertAudit
    @WeatherRecordId INT,
    @FieldName NVARCHAR(100),
    @OldValue NVARCHAR(100),
    @NewValue NVARCHAR(100),
    @ChangedByUserId NVARCHAR(450),
    @ChangedAt DATETIME
AS
BEGIN
INSERT INTO WeatherAudits (WeatherRecordId, FieldName, OldValue, NewValue, ChangedByUserId, ChangedAt)
VALUES (@WeatherRecordId, @FieldName, @OldValue, @NewValue, @ChangedByUserId, @ChangedAt)
END
GO