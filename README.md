# app-insights

## Build, run, test

```bash
cd AppInsights
dotnet restore
dotnet build
dotnet run --project src/Cli/Cli.csproj
```

On first run the app creates `data/appinsights.db` at the repository root and, if you confirm
the prompt, seeds it from `laptimes.json`. From there: pick a driver (currently scoped to the
Audi team), see their Q1/Q2/Q3 best times and final grid position, and optionally open "More
data" for the sector/theoretical-lap analysis. Every screen has a Back and/or Exit button.

```bash
cd AppInsights
dotnet test
```

Runs the full test suite — see [Testing approach](#testing-approach).

## Architecture and key design choices

This project follows the Clean Architecture pattern and is based on the template maintained by
Jason Taylor ([GitHub repository](https://github.com/jasontaylordev/cleanarchitecture)), although
it wasn't used in its complete form.

I've cherry-picked the features most relevant to this project's use case, keeping the original
structure, central package management, MediatR and testing patterns.

For the user interaction layer I chose [Terminal.Gui](https://github.com/tui-cs/Terminal.Gui), as
it provides a nice experience for console applications and is easy to set up and use.

In terms of data loading, there are several possible approaches — one being to query the JSON
file directly. I preferred to load the data once into a SQL database and then take advantage of a
relational database to work with the data in a properly structured way.

## Assumptions and trade-offs

There's a significant assumption about how the data is structured in the JSON file — if the
schema changes, the import logic will need to adapt. However, the domain model isn't directly
tied to the JSON structure.

## Why the qualifying performance analysis feature

With the data available in the dataset, there isn't a great deal to observe from a performance
analysis perspective. Lap time can be influenced by atmospheric conditions, tyre wear or engine
modes, and very few of those parameters are actually measured here.

In my opinion, a good comparison is analysing the mini-sector and sector data against a driver's
own times, or the best overall times, within the same qualifying session.

## Testing approach

I chose to only test the "Application" layer, as that's where the business logic resides,
covering edge cases and main flows to ensure we return the data we want.

This approach is simple, light to load and easy to maintain — we don't need the full complexity
of integration tests here.

## Future improvements

Given more time and a wider dataset, it would be interesting to implement proper corner-by-corner
analysis — tracing power delivery out of corners onto the straights, identifying weak approaches
into corners, and highlighting where performance could be maximised.

## How AI tools were used

This solution was built with Claude Code, used less as an autocomplete tool and more as a pairing
partner throughout the whole process.

Nowadays, writing code from scratch isn't what I want to focus on as a Senior Software Engineer —
I'd rather focus on the architectural choices, patterns and trade-offs.

I always aim for a simpler solution than an agentic tool would produce on its own, and that's been
part of my steering throughout this project.

I've been setting the guardrails, the boundaries and the design choices, while correcting
behaviours and enforcing best practices along the way.
