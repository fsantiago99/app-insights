using AppInsights.Cli.Theming;
using Terminal.Gui;

namespace AppInsights.Cli.Components;

internal static class ChoiceDialog
{
    public static int Show(string title, string message, params string[] choices)
    {
        var selected = -1;
        var buttons = new Button[choices.Length];

        for (var i = 0; i < choices.Length; i++)
        {
            var index = i;
            var button = new Button { Text = choices[i], ColorScheme = ButtonTheme.Scheme };
            button.Accepting += (_, e) =>
            {
                selected = index;
                e.Cancel = true;
                Terminal.Gui.Application.RequestStop();
            };
            buttons[i] = button;
        }

        using var dialog = new Dialog
        {
            Title = title,
            Text = message,
            TextAlignment = Alignment.Start,
            ButtonAlignment = Alignment.Center,
            ButtonAlignmentModes = AlignmentModes.AddSpaceBetweenItems,
            Buttons = buttons,
            ColorScheme = AppColorScheme.Scheme
        };

        Terminal.Gui.Application.Run(dialog);

        return selected;
    }
}
