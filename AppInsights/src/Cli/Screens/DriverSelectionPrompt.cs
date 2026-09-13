using AppInsights.Cli.Components;
using AppInsights.Cli.Theming;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace AppInsights.Cli.Screens;

public static class DriverSelectionPrompt
{
    public static string? SelectDriver(IReadOnlyList<string> driverCodes)
    {
        if (driverCodes.Count == 0)
        {
            return null;
        }

        string? selected = null;

        var window = new Window
        {
            Title = "Select Driver",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = AppColorScheme.Scheme
        };

        var logoLabel = new Label
        {
            X = Pos.Center(),
            Y = 1,
            Text = AudiLogo.Text,
            ColorScheme = new ColorScheme { Normal = new Attribute(AppColors.LogoMark, AppColors.Background) }
        };

        var messageLabel = new Label { X = Pos.Center(), Y = Pos.Bottom(logoLabel) + 1, Text = "Choose a driver:" };

        var buttonBar = new View
        {
            X = Pos.Center(),
            Y = Pos.Bottom(messageLabel) + 1,
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = true
        };

        Button? previousButton = null;
        foreach (var driverCode in driverCodes)
        {
            var button = new Button
            {
                Text = driverCode,
                X = previousButton is null ? 0 : Pos.Right(previousButton) + 1,
                Y = 0,
                ColorScheme = ButtonTheme.Scheme
            };
            button.Accepting += (_, e) =>
            {
                selected = driverCode;
                e.Cancel = true;
                Terminal.Gui.Application.RequestStop();
            };
            buttonBar.Add(button);
            previousButton = button;
        }

        var exitButton = new Button { Text = "Exit", X = Pos.AnchorEnd(), Y = Pos.AnchorEnd(), ColorScheme = ButtonTheme.Scheme };
        exitButton.Accepting += (_, e) =>
        {
            e.Cancel = true;
            TerminalSession.Terminate();
        };

        window.Add(logoLabel, messageLabel, buttonBar, exitButton);

        Terminal.Gui.Application.Run(window);

        return selected;
    }
}
