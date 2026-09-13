using AppInsights.Application;
using AppInsights.Application.Drivers.Queries.GetDriversByTeam;
using AppInsights.Application.Laps.Queries.GetBestLap;
using AppInsights.Application.Laps.Queries.GetQualifyingClassification;
using AppInsights.Application.Laps.Queries.GetQualifyingPerformanceAnalysis;
using AppInsights.Cli;
using AppInsights.Cli.Screens;
using AppInsights.Infrastructure;
using AppInsights.Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

const string TeamName = "Audi";
var qualifyingSegments = new[] { "Q1", "Q2", "Q3" };

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var repositoryRoot = FindRepositoryRoot(AppContext.BaseDirectory);
var dataDirectory = Path.Combine(repositoryRoot, "data");
Directory.CreateDirectory(dataDirectory);
builder.Configuration["ConnectionStrings:DefaultConnection"] = $"Data Source={Path.Combine(dataDirectory, "appinsights.db")}";

builder.Logging.ClearProviders();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
await initialiser.InitialiseAsync();

var seedDataPath = builder.Configuration["SeedDataPath"];
var resolvedSeedDataPath = seedDataPath is null ? null : Path.Combine(repositoryRoot, seedDataPath);

var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

TerminalSession.Run(async () =>
{
    if (!await initialiser.HasAnyLapsAsync() && DatabaseSetupPrompt.ConfirmInitialLoad())
    {
        await initialiser.SeedAsync(resolvedSeedDataPath);
    }

    var driverCodes = await mediator.Send(new GetDriversByTeamQuery(TeamName));

    while (true)
    {
        var selectedDriver = DriverSelectionPrompt.SelectDriver(driverCodes);
        if (selectedDriver is null)
        {
            break;
        }

        var q1 = await mediator.Send(new GetBestLapQuery(selectedDriver, "Q1"));
        var q2 = await mediator.Send(new GetBestLapQuery(selectedDriver, "Q2"));
        var q3 = await mediator.Send(new GetBestLapQuery(selectedDriver, "Q3"));
        var position = await mediator.Send(new GetQualifyingClassificationQuery(selectedDriver));

        var returnToDriverSelection = false;
        while (!returnToDriverSelection)
        {
            var action = QualifyingResultsPrompt.Show(selectedDriver, q1.LapTimeSeconds, q2.LapTimeSeconds, q3.LapTimeSeconds, position);
            switch (action)
            {
                case QualifyingResultsAction.MoreData:
                    var analyses = new List<QualifyingPerformanceAnalysisDto>();
                    foreach (var segment in qualifyingSegments)
                    {
                        analyses.Add(await mediator.Send(new GetQualifyingPerformanceAnalysisQuery(selectedDriver, segment)));
                    }
                    QualifyingPerformanceAnalysisPrompt.Show(selectedDriver, analyses);
                    break;
                case QualifyingResultsAction.Back:
                    returnToDriverSelection = true;
                    break;
                case QualifyingResultsAction.Exit:
                    return;
            }
        }
    }
});

Environment.Exit(0);

static string FindRepositoryRoot(string startDirectory)
{
    var directory = new DirectoryInfo(startDirectory);

    while (directory is not null)
    {
        if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    return startDirectory;
}
