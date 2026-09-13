using AppInsights.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Drivers.Queries.GetDriversByTeam;

public record GetDriversByTeamQuery(string TeamName) : IRequest<List<string>>;

public class GetDriversByTeamQueryHandler(IApplicationDbContext context) : IRequestHandler<GetDriversByTeamQuery, List<string>>
{
    public Task<List<string>> Handle(GetDriversByTeamQuery request, CancellationToken cancellationToken) =>
        context.Laps
            .Where(l => l.Team.Name == request.TeamName)
            .Select(l => l.Driver.Code)
            .Distinct()
            .OrderBy(code => code)
            .ToListAsync(cancellationToken);
}
