using AppInsights.Application.Common.Interfaces;
using AppInsights.Domain.Entities;
using AppInsights.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppInsights.Application.Laps.Commands.CreateLap;

public record CreateLapCommand : IRequest<int>
{
    public string DriverCode { get; init; } = string.Empty;

    public string? DriverCarNumber { get; init; }

    public string TeamName { get; init; } = string.Empty;

    public int LapNumber { get; init; }

    public int? Stint { get; init; }

    public TyreCompound Compound { get; init; }

    public int? TyreLifeLaps { get; init; }

    public double? LapTimeSeconds { get; init; }

    public double? Sector1Seconds { get; init; }

    public double? Sector2Seconds { get; init; }

    public double? Sector3Seconds { get; init; }
}

public class CreateLapCommandValidator : AbstractValidator<CreateLapCommand>
{
    public CreateLapCommandValidator()
    {
        RuleFor(x => x.DriverCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.TeamName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LapNumber).GreaterThan(0);
        RuleFor(x => x.Stint).GreaterThan(0).When(x => x.Stint is not null);
    }
}

public class CreateLapCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateLapCommand, int>
{
    public async Task<int> Handle(CreateLapCommand request, CancellationToken cancellationToken)
    {
        var driver = await context.Drivers.FirstOrDefaultAsync(d => d.Code == request.DriverCode, cancellationToken);
        if (driver is null)
        {
            driver = new Driver { Code = request.DriverCode, CarNumber = request.DriverCarNumber };
            context.Drivers.Add(driver);
        }

        var team = await context.Teams.FirstOrDefaultAsync(t => t.Name == request.TeamName, cancellationToken);
        if (team is null)
        {
            team = new Team { Name = request.TeamName };
            context.Teams.Add(team);
        }

        var entity = new Lap
        {
            Driver = driver,
            Team = team,
            LapNumber = request.LapNumber,
            Stint = request.Stint,
            Compound = request.Compound,
            TyreLifeLaps = request.TyreLifeLaps,
            LapTimeSeconds = request.LapTimeSeconds,
            Sector1Seconds = request.Sector1Seconds,
            Sector2Seconds = request.Sector2Seconds,
            Sector3Seconds = request.Sector3Seconds
        };

        context.Laps.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
