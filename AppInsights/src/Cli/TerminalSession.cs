using Terminal.Gui;

namespace AppInsights.Cli;

internal static class TerminalSession
{
    public static void Run(Func<Task> body)
    {
        Terminal.Gui.Application.Init(driverName: "NetDriver");
        Terminal.Gui.Application.Force16Colors = !TerminalSupportsTrueColor();
        try
        {
            var root = new Toplevel();
            root.Loaded += async (_, _) =>
            {
                try
                {
                    await body();
                }
                finally
                {
                    Terminal.Gui.Application.RequestStop();
                }
            };

            Terminal.Gui.Application.Run(root);
        }
        finally
        {
            Terminal.Gui.Application.Shutdown();
        }
    }

    public static void Terminate()
    {
        try
        {
            Terminal.Gui.Application.Shutdown();
        }
        finally
        {
            Environment.Exit(0);
        }
    }

    private static bool TerminalSupportsTrueColor()
    {
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");
        return colorTerm is "truecolor" or "24bit";
    }
}
