using AppInsights.Application.Laps.Queries.GetQualifyingPerformanceAnalysis;
using AppInsights.Application.UnitTests.Common;
using AppInsights.Domain.Entities;
using Xunit;

namespace AppInsights.Application.UnitTests.Laps.Queries.GetQualifyingPerformanceAnalysis;

public class GetQualifyingPerformanceAnalysisQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnActualBestLap_WithItsOwnSectorsAndMiniSectors()
    {
        using var context = TestDbContextFactory.Create();
        var driver = new Driver { Code = "HUL" };
        var team = new Team { Name = "Audi" };

        context.Laps.AddRange(
            new Lap
            {
                Driver = driver,
                Team = team,
                LapNumber = 1,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 81.5,
                Sector1Seconds = 27.0,
                Sector2Seconds = 27.0,
                Sector3Seconds = 27.5,
                MiniSector1 = "111",
                MiniSector2 = "222",
                MiniSector3 = "333",
                IsAccurate = true,
                TrackStatus = "1"
            },
            new Lap
            {
                Driver = driver,
                Team = team,
                LapNumber = 2,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 80.9,
                Sector1Seconds = 26.9,
                Sector2Seconds = 26.8,
                Sector3Seconds = 27.2,
                MiniSector1 = "444",
                MiniSector2 = "555",
                MiniSector3 = "666",
                IsAccurate = true,
                TrackStatus = "1"
            });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetQualifyingPerformanceAnalysisQueryHandler(context);
        var result = await handler.Handle(new GetQualifyingPerformanceAnalysisQuery("HUL", "Q1"), CancellationToken.None);

        Assert.NotNull(result.BestLap);
        Assert.Equal(80.9, result.BestLap.LapTimeSeconds);
        Assert.Equal(26.9, result.BestLap.Sector1Seconds);
        Assert.Equal("444", result.BestLap.MiniSector1);
    }

    [Fact]
    public async Task Handle_ShouldCombineDeletedLapSectors_IntoDriverTheoreticalBest()
    {
        using var context = TestDbContextFactory.Create();
        var driver = new Driver { Code = "HUL" };
        var team = new Team { Name = "Audi" };

        context.Laps.AddRange(
            new Lap
            {
                Driver = driver,
                Team = team,
                LapNumber = 1,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 80.9,
                Sector1Seconds = 27.0,
                Sector2Seconds = 26.8,
                Sector3Seconds = 27.1,
                IsAccurate = true,
                TrackStatus = "1"
            },
            new Lap
            {
                Driver = driver,
                Team = team,
                LapNumber = 2,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 81.5,
                Sector1Seconds = 26.0,
                Sector2Seconds = 28.0,
                Sector3Seconds = 27.5,
                IsAccurate = true,
                TrackStatus = "1",
                IsDeleted = true
            });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetQualifyingPerformanceAnalysisQueryHandler(context);
        var result = await handler.Handle(new GetQualifyingPerformanceAnalysisQuery("HUL", "Q1"), CancellationToken.None);

        Assert.Equal(26.0, result.DriverTheoreticalBest.Sector1Seconds);
        Assert.Equal(26.8, result.DriverTheoreticalBest.Sector2Seconds);
        Assert.Equal(27.1, result.DriverTheoreticalBest.Sector3Seconds);
        Assert.Equal(26.0 + 26.8 + 27.1, result.DriverTheoreticalBest.TheoreticalLapSeconds);
    }

    [Fact]
    public async Task Handle_ShouldCombineBestSectorsAcrossAllDrivers_RegardlessOfWhoSetThem()
    {
        using var context = TestDbContextFactory.Create();
        var team = new Team { Name = "Audi" };
        var driverA = new Driver { Code = "A" };
        var driverB = new Driver { Code = "B" };

        context.Laps.AddRange(
            new Lap
            {
                Driver = driverA,
                Team = team,
                LapNumber = 1,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 80.0,
                Sector1Seconds = 26.0,
                Sector2Seconds = 27.5,
                Sector3Seconds = 27.0,
                IsAccurate = true,
                TrackStatus = "1"
            },
            new Lap
            {
                Driver = driverB,
                Team = team,
                LapNumber = 1,
                QualifyingSegment = "Q1",
                LapTimeSeconds = 80.5,
                Sector1Seconds = 27.0,
                Sector2Seconds = 26.5,
                Sector3Seconds = 27.2,
                IsAccurate = true,
                TrackStatus = "1"
            });
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetQualifyingPerformanceAnalysisQueryHandler(context);
        var result = await handler.Handle(new GetQualifyingPerformanceAnalysisQuery("A", "Q1"), CancellationToken.None);

        Assert.Equal(26.0, result.AllDriversTheoreticalBest.Sector1Seconds);
        Assert.Equal(26.5, result.AllDriversTheoreticalBest.Sector2Seconds);
        Assert.Equal(27.0, result.AllDriversTheoreticalBest.Sector3Seconds);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullBestLap_WhenDriverHasNoValidLapInSegment()
    {
        using var context = TestDbContextFactory.Create();

        var handler = new GetQualifyingPerformanceAnalysisQueryHandler(context);
        var result = await handler.Handle(new GetQualifyingPerformanceAnalysisQuery("BOR", "Q3"), CancellationToken.None);

        Assert.Null(result.BestLap);
        Assert.Null(result.DriverTheoreticalBest.TheoreticalLapSeconds);
        Assert.Null(result.AllDriversTheoreticalBest.TheoreticalLapSeconds);
    }
}
