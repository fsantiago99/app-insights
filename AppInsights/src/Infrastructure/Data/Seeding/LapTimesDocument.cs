using System.Text.Json.Serialization;

namespace AppInsights.Infrastructure.Data.Seeding;

internal sealed class LapTimesDocument
{
    [JsonPropertyName("drv")]
    public string?[] Driver { get; init; } = [];

    [JsonPropertyName("dNum")]
    public string?[] DriverCarNumber { get; init; } = [];

    [JsonPropertyName("team")]
    public string?[] Team { get; init; } = [];

    [JsonPropertyName("lap")]
    public int?[] LapNumber { get; init; } = [];

    [JsonPropertyName("stint")]
    public int?[] Stint { get; init; } = [];

    [JsonPropertyName("compound")]
    public string?[] Compound { get; init; } = [];

    [JsonPropertyName("life")]
    public int?[] TyreLifeLaps { get; init; } = [];

    [JsonPropertyName("fresh")]
    public bool?[] IsFreshTyre { get; init; } = [];

    [JsonPropertyName("time")]
    public double?[] LapTimeSeconds { get; init; } = [];

    [JsonPropertyName("s1")]
    public double?[] Sector1Seconds { get; init; } = [];

    [JsonPropertyName("s2")]
    public double?[] Sector2Seconds { get; init; } = [];

    [JsonPropertyName("s3")]
    public double?[] Sector3Seconds { get; init; } = [];

    [JsonPropertyName("s1T")]
    public double?[] Sector1ElapsedSeconds { get; init; } = [];

    [JsonPropertyName("s2T")]
    public double?[] Sector2ElapsedSeconds { get; init; } = [];

    [JsonPropertyName("s3T")]
    public double?[] Sector3ElapsedSeconds { get; init; } = [];

    [JsonPropertyName("sesT")]
    public double?[] SessionElapsedSeconds { get; init; } = [];

    [JsonPropertyName("lST")]
    public double?[] LapStartElapsedSeconds { get; init; } = [];

    [JsonPropertyName("lSD")]
    public string?[] LapStartedAt { get; init; } = [];

    [JsonPropertyName("vi1")]
    public double?[] SpeedTrap1Kph { get; init; } = [];

    [JsonPropertyName("vi2")]
    public double?[] SpeedTrap2Kph { get; init; } = [];

    [JsonPropertyName("vfl")]
    public double?[] SpeedTrapFinishKph { get; init; } = [];

    [JsonPropertyName("vst")]
    public double?[] SpeedTrapStraightKph { get; init; } = [];

    [JsonPropertyName("ms1")]
    public string?[] MiniSector1 { get; init; } = [];

    [JsonPropertyName("ms2")]
    public string?[] MiniSector2 { get; init; } = [];

    [JsonPropertyName("ms3")]
    public string?[] MiniSector3 { get; init; } = [];

    [JsonPropertyName("qs")]
    public string?[] QualifyingSegment { get; init; } = [];

    [JsonPropertyName("pos")]
    public int?[] Position { get; init; } = [];

    [JsonPropertyName("status")]
    public string?[] TrackStatus { get; init; } = [];

    [JsonPropertyName("pb")]
    public bool?[] IsPersonalBest { get; init; } = [];

    [JsonPropertyName("pin")]
    public double?[] PitInElapsedSeconds { get; init; } = [];

    [JsonPropertyName("pout")]
    public double?[] PitOutElapsedSeconds { get; init; } = [];

    [JsonPropertyName("iacc")]
    public bool?[] IsAccurate { get; init; } = [];

    [JsonPropertyName("ff1G")]
    public bool?[] IsInterpolated { get; init; } = [];

    [JsonPropertyName("del")]
    public bool?[] IsDeleted { get; init; } = [];

    [JsonPropertyName("delR")]
    public string?[] DeletedReason { get; init; } = [];

    [JsonPropertyName("wT")]
    public double?[] WeatherElapsedSeconds { get; init; } = [];

    [JsonPropertyName("wAT")]
    public double?[] AirTemperatureCelsius { get; init; } = [];

    [JsonPropertyName("wH")]
    public double?[] HumidityPercent { get; init; } = [];

    [JsonPropertyName("wP")]
    public double?[] PressureMillibar { get; init; } = [];

    [JsonPropertyName("wR")]
    public bool?[] IsRaining { get; init; } = [];

    [JsonPropertyName("wTT")]
    public double?[] TrackTemperatureCelsius { get; init; } = [];

    [JsonPropertyName("wWD")]
    public double?[] WindDirectionDegrees { get; init; } = [];

    [JsonPropertyName("wWS")]
    public double?[] WindSpeedMetersPerSecond { get; init; } = [];
}
