using System.Text.Json;
using AppInsights.Domain.Entities;
using AppInsights.Domain.Enums;
using AppInsights.Infrastructure.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppInsights.Infrastructure.Data;

public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters =
        {
            new ResilientNullableDoubleConverter(),
            new ResilientNullableIntConverter(),
            new ResilientNullableBoolConverter(),
            new ResilientNullableStringConverter()
        }
    };

    public Task<bool> HasAnyLapsAsync() => context.Laps.AnyAsync();

    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync(string? lapTimesJsonPath = null)
    {
        try
        {
            await TrySeedAsync(lapTimesJsonPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync(string? lapTimesJsonPath)
    {
        if (await context.Laps.AnyAsync())
        {
            return;
        }

        if (lapTimesJsonPath is not null && File.Exists(lapTimesJsonPath))
        {
            await SeedFromLapTimesFileAsync(lapTimesJsonPath);
        }
    }

    private async Task SeedFromLapTimesFileAsync(string path)
    {
        LapTimesDocument document;
        await using (var stream = File.OpenRead(path))
        {
            document = await JsonSerializer.DeserializeAsync<LapTimesDocument>(stream, _serializerOptions)
                ?? new LapTimesDocument();
        }

        var count = new[] { document.Driver.Length, document.Team.Length, document.LapNumber.Length }.Max();

        var driverCarNumbers = new Dictionary<string, string?>();
        var teamNames = new HashSet<string>();

        for (var i = 0; i < count; i++)
        {
            var code = Get(document.Driver, i);
            if (!string.IsNullOrWhiteSpace(code) && !driverCarNumbers.ContainsKey(code))
            {
                driverCarNumbers[code] = Get(document.DriverCarNumber, i);
            }

            var team = Get(document.Team, i);
            if (!string.IsNullOrWhiteSpace(team))
            {
                teamNames.Add(team);
            }
        }

        var drivers = await GetOrCreateDriversAsync(driverCarNumbers);
        var teams = await GetOrCreateTeamsAsync(teamNames);

        await context.SaveChangesAsync(CancellationToken.None);

        var laps = new List<Lap>(count);
        var skipped = 0;

        for (var i = 0; i < count; i++)
        {
            var driverCode = Get(document.Driver, i);
            var teamName = Get(document.Team, i);
            var lapNumber = Get(document.LapNumber, i);

            if (string.IsNullOrWhiteSpace(driverCode) || !drivers.TryGetValue(driverCode, out var driver))
            {
                logger.LogWarning("Skipping lap row {Row} in {Path}: no driver code.", i, path);
                skipped++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(teamName) || !teams.TryGetValue(teamName, out var team))
            {
                logger.LogWarning("Skipping lap row {Row} in {Path} for driver {Driver}: no team name.", i, path, driverCode);
                skipped++;
                continue;
            }

            if (lapNumber is null)
            {
                logger.LogWarning("Skipping lap row {Row} in {Path} for driver {Driver}: no lap number.", i, path, driverCode);
                skipped++;
                continue;
            }

            laps.Add(new Lap
            {
                Driver = driver,
                Team = team,
                LapNumber = lapNumber.Value,
                Stint = Get(document.Stint, i),
                Compound = ParseCompound(Get(document.Compound, i)),
                TyreLifeLaps = Get(document.TyreLifeLaps, i),
                IsFreshTyre = Get(document.IsFreshTyre, i),
                LapTimeSeconds = Get(document.LapTimeSeconds, i),
                Sector1Seconds = Get(document.Sector1Seconds, i),
                Sector2Seconds = Get(document.Sector2Seconds, i),
                Sector3Seconds = Get(document.Sector3Seconds, i),
                Sector1ElapsedSeconds = Get(document.Sector1ElapsedSeconds, i),
                Sector2ElapsedSeconds = Get(document.Sector2ElapsedSeconds, i),
                Sector3ElapsedSeconds = Get(document.Sector3ElapsedSeconds, i),
                SessionElapsedSeconds = Get(document.SessionElapsedSeconds, i),
                LapStartElapsedSeconds = Get(document.LapStartElapsedSeconds, i),
                LapStartedAt = ParseTimestamp(Get(document.LapStartedAt, i)),
                SpeedTrap1Kph = Get(document.SpeedTrap1Kph, i),
                SpeedTrap2Kph = Get(document.SpeedTrap2Kph, i),
                SpeedTrapFinishKph = Get(document.SpeedTrapFinishKph, i),
                SpeedTrapStraightKph = Get(document.SpeedTrapStraightKph, i),
                MiniSector1 = Get(document.MiniSector1, i),
                MiniSector2 = Get(document.MiniSector2, i),
                MiniSector3 = Get(document.MiniSector3, i),
                QualifyingSegment = Get(document.QualifyingSegment, i),
                Position = Get(document.Position, i),
                TrackStatus = Get(document.TrackStatus, i),
                IsPersonalBest = Get(document.IsPersonalBest, i) ?? false,
                PitInElapsedSeconds = Get(document.PitInElapsedSeconds, i),
                PitOutElapsedSeconds = Get(document.PitOutElapsedSeconds, i),
                IsAccurate = Get(document.IsAccurate, i) ?? false,
                IsInterpolated = Get(document.IsInterpolated, i) ?? false,
                IsDeleted = Get(document.IsDeleted, i) ?? false,
                DeletedReason = Get(document.DeletedReason, i),
                Weather = new WeatherReading
                {
                    ElapsedSeconds = Get(document.WeatherElapsedSeconds, i),
                    AirTemperatureCelsius = Get(document.AirTemperatureCelsius, i),
                    HumidityPercent = Get(document.HumidityPercent, i),
                    PressureMillibar = Get(document.PressureMillibar, i),
                    IsRaining = Get(document.IsRaining, i),
                    TrackTemperatureCelsius = Get(document.TrackTemperatureCelsius, i),
                    WindDirectionDegrees = Get(document.WindDirectionDegrees, i),
                    WindSpeedMetersPerSecond = Get(document.WindSpeedMetersPerSecond, i)
                }
            });
        }

        context.Laps.AddRange(laps);
        await context.SaveChangesAsync(CancellationToken.None);

        logger.LogInformation(
            "Seeded {LapCount} laps ({SkippedCount} skipped) for {DriverCount} drivers and {TeamCount} teams from {Path}",
            laps.Count, skipped, drivers.Count, teams.Count, path);
    }

    private async Task<Dictionary<string, Driver>> GetOrCreateDriversAsync(Dictionary<string, string?> carNumbersByCode)
    {
        var codes = carNumbersByCode.Keys.ToList();

        var drivers = await context.Drivers
            .Where(d => codes.Contains(d.Code))
            .ToDictionaryAsync(d => d.Code);

        foreach (var (code, carNumber) in carNumbersByCode)
        {
            if (drivers.ContainsKey(code))
            {
                continue;
            }

            var driver = new Driver { Code = code, CarNumber = carNumber };
            context.Drivers.Add(driver);
            drivers[code] = driver;
        }

        return drivers;
    }

    private async Task<Dictionary<string, Team>> GetOrCreateTeamsAsync(HashSet<string> names)
    {
        var teams = await context.Teams
            .Where(t => names.Contains(t.Name))
            .ToDictionaryAsync(t => t.Name);

        foreach (var name in names)
        {
            if (teams.ContainsKey(name))
            {
                continue;
            }

            var team = new Team { Name = name };
            context.Teams.Add(team);
            teams[name] = team;
        }

        return teams;
    }

    private static TValue? Get<TValue>(TValue?[] source, int index) => index < source.Length ? source[index] : default;

    private static TyreCompound ParseCompound(string? value) => value?.Trim().ToUpperInvariant() switch
    {
        "SOFT" => TyreCompound.Soft,
        "MEDIUM" => TyreCompound.Medium,
        "HARD" => TyreCompound.Hard,
        "INTERMEDIATE" => TyreCompound.Intermediate,
        "WET" => TyreCompound.Wet,
        _ => TyreCompound.Unknown
    };

    private static DateTime? ParseTimestamp(string? value) =>
        DateTime.TryParse(value, out var timestamp) ? timestamp : null;
}
