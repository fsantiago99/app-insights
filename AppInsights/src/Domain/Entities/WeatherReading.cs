namespace AppInsights.Domain.Entities;

public class WeatherReading
{
    public int LapId { get; set; }

    public Lap Lap { get; set; } = null!;

    public double? ElapsedSeconds { get; set; }

    public double? AirTemperatureCelsius { get; set; }

    public double? HumidityPercent { get; set; }

    public double? PressureMillibar { get; set; }

    public bool? IsRaining { get; set; }

    public double? TrackTemperatureCelsius { get; set; }

    public double? WindDirectionDegrees { get; set; }

    public double? WindSpeedMetersPerSecond { get; set; }
}
