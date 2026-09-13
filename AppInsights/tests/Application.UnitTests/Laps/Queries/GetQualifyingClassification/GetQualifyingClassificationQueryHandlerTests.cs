using AppInsights.Application.Laps.Queries.GetQualifyingClassification;
using AppInsights.Application.UnitTests.Common;
using AppInsights.Domain.Entities;
using AppInsights.Infrastructure.Data;
using Xunit;

namespace AppInsights.Application.UnitTests.Laps.Queries.GetQualifyingClassification;

public class GetQualifyingClassificationQueryHandlerTests
{
    private static async Task<ApplicationDbContext> BuildTwentyDriverGridAsync()
    {
        var context = TestDbContextFactory.Create();
        var team = new Team { Name = "Audi" };

        for (var i = 1; i <= 20; i++)
        {
            var driver = new Driver { Code = $"D{i:00}" };
            var q1Time = 80.0 + i * 0.1;
            context.Laps.Add(ValidLap(driver, team, 1, "Q1", q1Time));

            if (i <= 16)
            {
                var q2Time = 79.0 + i * 0.1;
                context.Laps.Add(ValidLap(driver, team, 2, "Q2", q2Time));
            }

            if (i <= 10)
            {
                var q3Time = 78.0 + i * 0.1;
                context.Laps.Add(ValidLap(driver, team, 3, "Q3", q3Time));
            }
        }

        await context.SaveChangesAsync(CancellationToken.None);
        return context;
    }

    private static Lap ValidLap(Driver driver, Team team, int lapNumber, string segment, double timeSeconds) => new()
    {
        Driver = driver,
        Team = team,
        LapNumber = lapNumber,
        QualifyingSegment = segment,
        LapTimeSeconds = timeSeconds,
        IsAccurate = true,
        TrackStatus = "1"
    };

    [Theory]
    [InlineData("D01", 1)]
    [InlineData("D10", 10)]
    [InlineData("D11", 11)]
    [InlineData("D16", 16)]
    [InlineData("D17", 17)]
    [InlineData("D20", 20)]
    public async Task Handle_ShouldClassifyDriver_AccordingToEliminationFormat(string driverCode, int expectedPosition)
    {
        using var context = await BuildTwentyDriverGridAsync();

        var handler = new GetQualifyingClassificationQueryHandler(context);
        var position = await handler.Handle(new GetQualifyingClassificationQuery(driverCode), CancellationToken.None);

        Assert.Equal(expectedPosition, position);
    }

    [Fact]
    public async Task Handle_ShouldNotPromoteDriverToQ3_JustForHavingTheFastestQ2Time()
    {
        using var context = TestDbContextFactory.Create();
        var team = new Team { Name = "Audi" };
        var driverA = new Driver { Code = "A" };
        var driverB = new Driver { Code = "B" };
        var driverC = new Driver { Code = "C" };

        context.Laps.AddRange(
            ValidLap(driverA, team, 1, "Q1", 80.0), ValidLap(driverA, team, 2, "Q2", 79.0),
            ValidLap(driverB, team, 1, "Q1", 80.1), ValidLap(driverB, team, 2, "Q2", 79.5), ValidLap(driverB, team, 3, "Q3", 78.0),
            ValidLap(driverC, team, 1, "Q1", 80.2), ValidLap(driverC, team, 2, "Q2", 79.8));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetQualifyingClassificationQueryHandler(context);

        Assert.Equal(1, await handler.Handle(new GetQualifyingClassificationQuery("B"), CancellationToken.None));
        Assert.Equal(2, await handler.Handle(new GetQualifyingClassificationQuery("A"), CancellationToken.None));
        Assert.Equal(3, await handler.Handle(new GetQualifyingClassificationQuery("C"), CancellationToken.None));
    }
}
