using AppInsights.Application.Drivers.Queries.GetDriversByTeam;
using AppInsights.Application.UnitTests.Common;
using AppInsights.Domain.Entities;
using Xunit;

namespace AppInsights.Application.UnitTests.Drivers.Queries.GetDriversByTeam;

public class GetDriversByTeamQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnDistinctDriverCodes_ForRequestedTeam()
    {
        using var context = TestDbContextFactory.Create();
        var audi = new Team { Name = "Audi" };
        var mercedes = new Team { Name = "Mercedes" };
        var hulkenberg = new Driver { Code = "HUL" };
        var bortoleto = new Driver { Code = "BOR" };
        var russell = new Driver { Code = "RUS" };

        context.Laps.AddRange(
            new Lap { Driver = hulkenberg, Team = audi, LapNumber = 1, QualifyingSegment = "Q1" },
            new Lap { Driver = hulkenberg, Team = audi, LapNumber = 2, QualifyingSegment = "Q1" },
            new Lap { Driver = bortoleto, Team = audi, LapNumber = 1, QualifyingSegment = "Q1" },
            new Lap { Driver = russell, Team = mercedes, LapNumber = 1, QualifyingSegment = "Q1" });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetDriversByTeamQueryHandler(context);
        var result = await handler.Handle(new GetDriversByTeamQuery("Audi"), CancellationToken.None);

        Assert.Equal(["BOR", "HUL"], result);
    }
}
