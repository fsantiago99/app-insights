using System.Linq.Expressions;
using AppInsights.Application.Common.Interfaces;
using AppInsights.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Laps.Queries.GetQualifyingPerformanceAnalysis;

public record GetQualifyingPerformanceAnalysisQuery(string DriverCode, string QualifyingSegment)
    : IRequest<QualifyingPerformanceAnalysisDto>;

public record QualifyingPerformanceAnalysisDto(
    string QualifyingSegment,
    BestLapSectorsDto? BestLap,
    SectorTimesDto DriverTheoreticalBest,
    SectorTimesDto AllDriversTheoreticalBest);

public record BestLapSectorsDto(
    double? LapTimeSeconds,
    double? Sector1Seconds,
    double? Sector2Seconds,
    double? Sector3Seconds,
    string? MiniSector1,
    string? MiniSector2,
    string? MiniSector3);

public record SectorTimesDto(double? Sector1Seconds, double? Sector2Seconds, double? Sector3Seconds)
{
    public double? TheoreticalLapSeconds => Sector1Seconds is null || Sector2Seconds is null || Sector3Seconds is null
        ? null
        : Sector1Seconds + Sector2Seconds + Sector3Seconds;
}

public class GetQualifyingPerformanceAnalysisQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetQualifyingPerformanceAnalysisQuery, QualifyingPerformanceAnalysisDto>
{
    public async Task<QualifyingPerformanceAnalysisDto> Handle(GetQualifyingPerformanceAnalysisQuery request, CancellationToken cancellationToken)
    {
        var bestLap = await context.Laps
            .Where(l => l.Driver.Code == request.DriverCode && l.QualifyingSegment == request.QualifyingSegment)
            .Where(ValidLapSpecification.IsValid)
            .OrderBy(l => l.LapTimeSeconds)
            .Select(l => new BestLapSectorsDto(
                l.LapTimeSeconds, l.Sector1Seconds, l.Sector2Seconds, l.Sector3Seconds,
                l.MiniSector1, l.MiniSector2, l.MiniSector3))
            .FirstOrDefaultAsync(cancellationToken);

        var driverTheoreticalBest = await GetBestSectorsAsync(
            l => l.Driver.Code == request.DriverCode && l.QualifyingSegment == request.QualifyingSegment,
            cancellationToken);

        var allDriversTheoreticalBest = await GetBestSectorsAsync(
            l => l.QualifyingSegment == request.QualifyingSegment,
            cancellationToken);

        return new QualifyingPerformanceAnalysisDto(request.QualifyingSegment, bestLap, driverTheoreticalBest, allDriversTheoreticalBest);
    }

    private async Task<SectorTimesDto> GetBestSectorsAsync(
        Expression<Func<Lap, bool>> scope, CancellationToken cancellationToken)
    {
        var laps = context.Laps.Where(scope).Where(ValidLapSpecification.IsValidForTheoreticalBest);

        var sector1 = await laps.Select(l => l.Sector1Seconds).MinAsync(cancellationToken);
        var sector2 = await laps.Select(l => l.Sector2Seconds).MinAsync(cancellationToken);
        var sector3 = await laps.Select(l => l.Sector3Seconds).MinAsync(cancellationToken);

        return new SectorTimesDto(sector1, sector2, sector3);
    }
}
