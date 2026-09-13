using AppInsights.Domain.Common;
using AppInsights.Domain.Enums;

namespace AppInsights.Domain.Entities;

public class Lap : BaseAuditableEntity
{
    public int DriverId { get; set; }

    public Driver Driver { get; set; } = null!;

    public int TeamId { get; set; }

    public Team Team { get; set; } = null!;

    public int LapNumber { get; set; }

    public int? Stint { get; set; }

    public TyreCompound Compound { get; set; }

    public int? TyreLifeLaps { get; set; }

    public bool? IsFreshTyre { get; set; }

    public double? LapTimeSeconds { get; set; }

    public double? Sector1Seconds { get; set; }

    public double? Sector2Seconds { get; set; }

    public double? Sector3Seconds { get; set; }

    public double? Sector1ElapsedSeconds { get; set; }

    public double? Sector2ElapsedSeconds { get; set; }

    public double? Sector3ElapsedSeconds { get; set; }

    public double? SessionElapsedSeconds { get; set; }

    public double? LapStartElapsedSeconds { get; set; }

    public DateTime? LapStartedAt { get; set; }

    public double? SpeedTrap1Kph { get; set; }

    public double? SpeedTrap2Kph { get; set; }

    public double? SpeedTrapFinishKph { get; set; }

    public double? SpeedTrapStraightKph { get; set; }

    public string? MiniSector1 { get; set; }

    public string? MiniSector2 { get; set; }

    public string? MiniSector3 { get; set; }

    public string? QualifyingSegment { get; set; }

    public int? Position { get; set; }

    public string? TrackStatus { get; set; }

    public bool IsPersonalBest { get; set; }

    public double? PitInElapsedSeconds { get; set; }

    public double? PitOutElapsedSeconds { get; set; }

    public bool IsAccurate { get; set; }

    public bool IsInterpolated { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedReason { get; set; }

    public WeatherReading? Weather { get; set; }

    public string FormattedLapTime => LapTimeSeconds is null
        ? "-"
        : TimeSpan.FromSeconds(LapTimeSeconds.Value).ToString(@"m\:ss\.fff");
}
