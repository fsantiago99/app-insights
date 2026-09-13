using AppInsights.Application.Laps.Queries.GetBestLap;
using AppInsights.Application.UnitTests.Common;
using AppInsights.Domain.Entities;
using Xunit;

namespace AppInsights.Application.UnitTests.Laps.Queries.GetBestLap;

public class GetBestLapQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFastestValidLap_ForDriverAndSegment()
    {
        using var context = TestDbContextFactory.Create();
        var driver = new Driver { Code = "HUL" };
        var team = new Team { Name = "Audi" };

        context.Laps.AddRange(
            new Lap { Driver = driver, Team = team, LapNumber = 1, QualifyingSegment = "Q1", LapTimeSeconds = 81.5, IsAccurate = true, TrackStatus = "1" },
            new Lap { Driver = driver, Team = team, LapNumber = 2, QualifyingSegment = "Q1", LapTimeSeconds = 80.9, IsAccurate = true, TrackStatus = "1" });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetBestLapQueryHandler(context);
        var result = await handler.Handle(new GetBestLapQuery("HUL", "Q1"), CancellationToken.None);

        Assert.Equal(80.9, result.LapTimeSeconds);
        Assert.Equal("Q1", result.QualifyingSegment);
    }

    [Theory]
    [InlineData(true, false, "1")]
    [InlineData(false, false, "1")]
    [InlineData(false, true, "12")]
    public async Task Handle_ShouldIgnoreInvalidLaps(bool isDeleted, bool isAccurate, string trackStatus)
    {
        using var context = TestDbContextFactory.Create();
        var driver = new Driver { Code = "BOR" };
        var team = new Team { Name = "Audi" };

        context.Laps.Add(new Lap
        {
            Driver = driver,
            Team = team,
            LapNumber = 1,
            QualifyingSegment = "Q1",
            LapTimeSeconds = 79.0,
            IsDeleted = isDeleted,
            IsAccurate = isAccurate,
            TrackStatus = trackStatus
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetBestLapQueryHandler(context);
        var result = await handler.Handle(new GetBestLapQuery("BOR", "Q1"), CancellationToken.None);

        Assert.Null(result.LapTimeSeconds);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullLapTime_WhenDriverHasNoLapsInSegment()
    {
        using var context = TestDbContextFactory.Create();

        var handler = new GetBestLapQueryHandler(context);
        var result = await handler.Handle(new GetBestLapQuery("BOR", "Q3"), CancellationToken.None);

        Assert.Null(result.LapTimeSeconds);
        Assert.Equal("Q3", result.QualifyingSegment);
    }
}
