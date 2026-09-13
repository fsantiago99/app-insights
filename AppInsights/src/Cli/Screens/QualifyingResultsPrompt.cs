using AppInsights.Cli.Theming;
using Terminal.Gui;

namespace AppInsights.Cli.Screens;

public enum QualifyingResultsAction
{
    Back,
    MoreData,
    Exit
}

public static class QualifyingResultsPrompt
{
    public static QualifyingResultsAction Show(string driverCode, double? q1, double? q2, double? q3, int position)
    {
        var message =
            $"Driver: {driverCode}\n" +
            $"Q1: {Format(q1)}\n" +
            $"Q2: {Format(q2)}\n" +
            $"Q3: {Format(q3)}\n" +
            $"\n" +
            $"Final Position: P{position}";

        var action = QualifyingResultsAction.Exit;

        var window = new Window
        {
            Title = "Qualifying Result",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = AppColorScheme.Scheme
        };

        var dataLabel = new Label
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            TextAlignment = Alignment.Start,
            Text = message
        };

        var backButton = new Button { Text = "Back", X = 0, Y = 0, ColorScheme = ButtonTheme.Scheme };
        var moreDataButton = new Button { Text = "More data", X = Pos.Right(backButton) + 1, Y = 0, ColorScheme = ButtonTheme.Scheme };
        var exitButton = new Button { Text = "Exit", X = Pos.Right(moreDataButton) + 1, Y = 0, ColorScheme = ButtonTheme.Scheme };

        backButton.Accepting += (_, e) =>
        {
            action = QualifyingResultsAction.Back;
            e.Cancel = true;
            Terminal.Gui.Application.RequestStop();
        };
        moreDataButton.Accepting += (_, e) =>
        {
            action = QualifyingResultsAction.MoreData;
            e.Cancel = true;
            Terminal.Gui.Application.RequestStop();
        };
        exitButton.Accepting += (_, e) =>
        {
            e.Cancel = true;
            TerminalSession.Terminate();
        };

        var buttonBar = new View
        {
            X = Pos.AnchorEnd(),
            Y = Pos.AnchorEnd(),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = true
        };
        buttonBar.Add(backButton, moreDataButton, exitButton);

        window.Add(dataLabel, buttonBar);

        Terminal.Gui.Application.Run(window);

        return action;
    }

    private static string Format(double? seconds) =>
        seconds is null ? "-" : TimeSpan.FromSeconds(seconds.Value).ToString(@"m\:ss\.fff");
}
