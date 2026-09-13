using AppInsights.Application.Laps.Commands.CreateLap;
using AppInsights.Domain.Enums;
using Xunit;

namespace AppInsights.Application.UnitTests.Laps.Commands;

public class CreateLapCommandValidatorTests
{
    private readonly CreateLapCommandValidator _validator = new();

    [Fact]
    public void ShouldHaveError_WhenDriverCodeIsEmpty()
    {
        var command = new CreateLapCommand { DriverCode = string.Empty, TeamName = "Ferrari", LapNumber = 1, Stint = 1 };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLapCommand.DriverCode));
    }

    [Fact]
    public void ShouldHaveError_WhenLapNumberIsNotPositive()
    {
        var command = new CreateLapCommand { DriverCode = "VER", TeamName = "Red Bull Racing", LapNumber = 0, Stint = 1 };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLapCommand.LapNumber));
    }

    [Fact]
    public void ShouldNotHaveError_WhenCommandIsValid()
    {
        var command = new CreateLapCommand
        {
            DriverCode = "VER",
            TeamName = "Red Bull Racing",
            LapNumber = 1,
            Stint = 1,
            Compound = TyreCompound.Soft
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
