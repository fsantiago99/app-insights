using AppInsights.Application.Laps.Commands.CreateLap;
using AppInsights.Application.UnitTests.Common;
using AppInsights.Domain.Enums;
using Xunit;

namespace AppInsights.Application.UnitTests.Laps.Commands;

public class CreateLapCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistLap_AndReturnItsId()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CreateLapCommandHandler(context);

        var command = new CreateLapCommand
        {
            DriverCode = "VER",
            DriverCarNumber = "1",
            TeamName = "Red Bull Racing",
            LapNumber = 1,
            Stint = 1,
            Compound = TyreCompound.Soft,
            TyreLifeLaps = 1,
            LapTimeSeconds = 91.234
        };

        var id = await handler.Handle(command, CancellationToken.None);

        var lap = await context.Laps.FindAsync(id);

        Assert.NotNull(lap);
        Assert.Equal("VER", (await context.Drivers.FindAsync(lap!.DriverId))?.Code);
        Assert.Equal("Red Bull Racing", (await context.Teams.FindAsync(lap.TeamId))?.Name);
        Assert.Equal(1, lap.LapNumber);
    }
}
