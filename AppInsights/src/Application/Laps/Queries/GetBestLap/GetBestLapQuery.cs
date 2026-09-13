using AppInsights.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Laps.Queries.GetBestLap;

public record GetBestLapQuery(string DriverCode, string QualifyingSegment) : IRequest<BestLapDto>;

public record BestLapDto(double? LapTimeSeconds, string QualifyingSegment);

public class GetBestLapQueryHandler(IApplicationDbContext context) : IRequestHandler<GetBestLapQuery, BestLapDto>
{
    public async Task<BestLapDto> Handle(GetBestLapQuery request, CancellationToken cancellationToken)
    {
        var bestLapTimeSeconds = await context.Laps
            .Where(l => l.Driver.Code == request.DriverCode && l.QualifyingSegment == request.QualifyingSegment)
            .Where(ValidLapSpecification.IsValid)
            .Select(l => l.LapTimeSeconds)
            .MinAsync(cancellationToken);

        return new BestLapDto(bestLapTimeSeconds, request.QualifyingSegment);
    }
}
