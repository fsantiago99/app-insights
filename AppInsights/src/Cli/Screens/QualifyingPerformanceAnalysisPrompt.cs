using System.Globalization;
using System.Text;
using AppInsights.Application.Laps.Queries.GetQualifyingPerformanceAnalysis;
using AppInsights.Cli.Components;
using AppInsights.Cli.Theming;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace AppInsights.Cli.Screens;

public static class QualifyingPerformanceAnalysisPrompt
{
    private static readonly string[] _rowLabels = ["Best Lap", "Driver TBL", "TBL"];

    public static void Show(string driverCode, IReadOnlyList<QualifyingPerformanceAnalysisDto> analyses)
    {
        var window = new Window
        {
            Title = $"Qualifying Performance Analysis ({driverCode})",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = AppColorScheme.Scheme
        };

        var sessions = analyses.Select(Format).ToList();
        var widths = ComputeColumnWidths(sessions);

        var row = 0;

        void AddLine((string, Attribute?)[] segments)
        {
            window.Add(new RichTextLine(segments) { X = 2, Y = row });
            row++;
        }

        for (var i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];

            AddLine(BorderLine('┌', '┬', '┐', widths));
            AddLine(DataRow([session.Segment, "S1", "S2", "S3", "Lap Time"], widths));

            if (session.MiniSector1 is not null || session.MiniSector2 is not null || session.MiniSector3 is not null)
            {
                AddLine(MiniSectorRow(session, widths));
            }

            AddLine(DataRow(["Best Lap", session.BestS1, session.BestS2, session.BestS3, session.BestLapTime], widths));
            AddLine(DataRow(["Driver TBL", session.DriverS1, session.DriverS2, session.DriverS3, session.DriverLapTime], widths));
            AddLine(DataRow(["TBL", session.FieldS1, session.FieldS2, session.FieldS3, session.FieldLapTime], widths));
            AddLine(BorderLine('└', '┴', '┘', widths));

            if (i < sessions.Count - 1)
            {
                row++;
            }
        }

        var backButton = new Button { Text = "Back", X = Pos.AnchorEnd(), Y = Pos.AnchorEnd(), ColorScheme = ButtonTheme.Scheme };
        backButton.Accepting += (_, e) =>
        {
            e.Cancel = true;
            Terminal.Gui.Application.RequestStop();
        };
        window.Add(backButton);

        Terminal.Gui.Application.Run(window);
    }

    private static int[] ComputeColumnWidths(IReadOnlyList<FormattedSession> sessions)
    {
        var labelWidth = _rowLabels.Concat(sessions.Select(s => s.Segment)).Max(s => s.Length);
        var s1Width = ColumnWidth("S1", sessions.Select(s => (string?)s.MiniSector1), sessions.SelectMany(s => new[] { s.BestS1, s.DriverS1, s.FieldS1 }));
        var s2Width = ColumnWidth("S2", sessions.Select(s => (string?)s.MiniSector2), sessions.SelectMany(s => new[] { s.BestS2, s.DriverS2, s.FieldS2 }));
        var s3Width = ColumnWidth("S3", sessions.Select(s => (string?)s.MiniSector3), sessions.SelectMany(s => new[] { s.BestS3, s.DriverS3, s.FieldS3 }));
        var lapTimeWidth = ColumnWidth("Lap Time", [], sessions.SelectMany(s => new[] { s.BestLapTime, s.DriverLapTime, s.FieldLapTime }));
        return [labelWidth, s1Width, s2Width, s3Width, lapTimeWidth];
    }

    private static int ColumnWidth(string header, IEnumerable<string?> miniSectorCodes, IEnumerable<string> values) =>
        new[] { header.Length }.Concat(miniSectorCodes.Select(c => c?.Length ?? 0)).Concat(values.Select(v => v.Length)).Max();

    private static (string, Attribute?)[] BorderLine(char left, char mid, char right, IReadOnlyList<int> widths)
    {
        var text = new StringBuilder().Append(left);
        for (var i = 0; i < widths.Count; i++)
        {
            text.Append('─', widths[i] + 2).Append(i == widths.Count - 1 ? right : mid);
        }

        return [(text.ToString(), null)];
    }

    private static (string, Attribute?)[] DataRow(IReadOnlyList<string> cells, IReadOnlyList<int> widths)
    {
        var text = new StringBuilder().Append('│');
        for (var i = 0; i < cells.Count; i++)
        {
            text.Append(' ').Append(Center(cells[i], widths[i])).Append(' ').Append('│');
        }

        return [(text.ToString(), null)];
    }

    private static (string, Attribute?)[] MiniSectorRow(FormattedSession session, IReadOnlyList<int> widths)
    {
        var segments = new List<(string, Attribute?)>
        {
            ("│ ", null),
            (new string(' ', widths[0]), null),
            (" │ ", null)
        };
        segments.AddRange(CenteredMiniSectorCells(session.MiniSector1, widths[1]));
        segments.Add((" │ ", null));
        segments.AddRange(CenteredMiniSectorCells(session.MiniSector2, widths[2]));
        segments.Add((" │ ", null));
        segments.AddRange(CenteredMiniSectorCells(session.MiniSector3, widths[3]));
        segments.Add((" │ ", null));
        segments.Add((new string(' ', widths[4]), null));
        segments.Add((" │", null));
        return [.. segments];
    }

    private static IEnumerable<(string, Attribute?)> CenteredMiniSectorCells(string? code, int width)
    {
        var length = code?.Length ?? 0;
        var leftPad = Math.Max(0, (width - length) / 2);
        var rightPad = Math.Max(0, width - length - leftPad);

        if (leftPad > 0)
        {
            yield return (new string(' ', leftPad), null);
        }

        if (code is not null)
        {
            foreach (var code1 in code)
            {
                yield return (" ", MiniSectorPalette.GetAttribute(code1));
            }
        }

        if (rightPad > 0)
        {
            yield return (new string(' ', rightPad), null);
        }
    }

    private static string Center(string text, int width)
    {
        var pad = Math.Max(0, width - text.Length);
        var left = pad / 2;
        var right = pad - left;
        return new string(' ', left) + text + new string(' ', right);
    }

    private static FormattedSession Format(QualifyingPerformanceAnalysisDto analysis) => new(
        analysis.QualifyingSegment,
        analysis.BestLap?.MiniSector1,
        analysis.BestLap?.MiniSector2,
        analysis.BestLap?.MiniSector3,
        FormatSector(analysis.BestLap?.Sector1Seconds),
        FormatSector(analysis.BestLap?.Sector2Seconds),
        FormatSector(analysis.BestLap?.Sector3Seconds),
        FormatTime(analysis.BestLap?.LapTimeSeconds),
        FormatSector(analysis.DriverTheoreticalBest.Sector1Seconds),
        FormatSector(analysis.DriverTheoreticalBest.Sector2Seconds),
        FormatSector(analysis.DriverTheoreticalBest.Sector3Seconds),
        FormatTime(analysis.DriverTheoreticalBest.TheoreticalLapSeconds),
        FormatSector(analysis.AllDriversTheoreticalBest.Sector1Seconds),
        FormatSector(analysis.AllDriversTheoreticalBest.Sector2Seconds),
        FormatSector(analysis.AllDriversTheoreticalBest.Sector3Seconds),
        FormatTime(analysis.AllDriversTheoreticalBest.TheoreticalLapSeconds));

    private static string FormatTime(double? seconds) =>
        seconds is null ? "-" : TimeSpan.FromSeconds(seconds.Value).ToString(@"m\:ss\.fff");

    private static string FormatSector(double? seconds) =>
        seconds is null ? "-" : seconds.Value.ToString("0.000", CultureInfo.InvariantCulture);

    private sealed record FormattedSession(
        string Segment,
        string? MiniSector1,
        string? MiniSector2,
        string? MiniSector3,
        string BestS1,
        string BestS2,
        string BestS3,
        string BestLapTime,
        string DriverS1,
        string DriverS2,
        string DriverS3,
        string DriverLapTime,
        string FieldS1,
        string FieldS2,
        string FieldS3,
        string FieldLapTime);
}
