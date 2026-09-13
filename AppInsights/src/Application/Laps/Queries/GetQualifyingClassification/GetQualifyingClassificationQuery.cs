using AppInsights.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Laps.Queries.GetQualifyingClassification;

public record GetQualifyingClassificationQuery(string DriverCode) : IRequest<int>;

public class GetQualifyingClassificationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetQualifyingClassificationQuery, int>
{
    public async Task<int> Handle(GetQualifyingClassificationQuery request, CancellationToken cancellationToken)
    {
        var knownDrivers = await context.Drivers.Select(d => d.Code).ToListAsync(cancellationToken);
        var q2Participants = await GetParticipantsAsync("Q2", cancellationToken);
        var q3Participants = await GetParticipantsAsync("Q3", cancellationToken);

        var q1Eliminated = knownDrivers.Except(q2Participants).ToList();
        var q2Eliminated = q2Participants.Except(q3Participants).ToList();

        var positions = new Dictionary<string, int>();

        var q3Order = await RankByBestLapAsync(q3Participants, "Q3", cancellationToken);
        AssignPositions(positions, q3Order, startingPosition: 1);

        var q2Order = await RankByBestLapAsync(q2Eliminated, "Q2", cancellationToken);
        AssignPositions(positions, q2Order, startingPosition: q3Order.Count + 1);

        var q1Order = await RankByBestLapAsync(q1Eliminated, "Q1", cancellationToken);
        AssignPositions(positions, q1Order, startingPosition: q3Order.Count + q2Order.Count + 1);

        return positions.TryGetValue(request.DriverCode, out var position) ? position : knownDrivers.Count;
    }

    private async Task<List<string>> GetParticipantsAsync(string qualifyingSegment, CancellationToken cancellationToken) =>
        await context.Laps
            .Where(l => l.QualifyingSegment == qualifyingSegment)
            .Select(l => l.Driver.Code)
            .Distinct()
            .ToListAsync(cancellationToken);

    private async Task<List<string>> RankByBestLapAsync(IReadOnlyCollection<string> driverCodes, string qualifyingSegment, CancellationToken cancellationToken)
    {
        var bestLapTimes = await context.Laps
            .Where(l => driverCodes.Contains(l.Driver.Code) && l.QualifyingSegment == qualifyingSegment)
            .Where(ValidLapSpecification.IsValid)
            .GroupBy(l => l.Driver.Code)
            .Select(g => new { DriverCode = g.Key, BestLapTimeSeconds = g.Min(l => l.LapTimeSeconds) })
            .ToDictionaryAsync(x => x.DriverCode, x => x.BestLapTimeSeconds, cancellationToken);

        return [.. driverCodes
            .OrderBy(code => bestLapTimes.GetValueOrDefault(code) ?? double.MaxValue)
            .ThenBy(code => code)];
    }

    private static void AssignPositions(Dictionary<string, int> positions, IEnumerable<string> orderedDriverCodes, int startingPosition)
    {
        var position = startingPosition;
        foreach (var driverCode in orderedDriverCodes)
        {
            positions[driverCode] = position++;
        }
    }
}
