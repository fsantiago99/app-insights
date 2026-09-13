using AppInsights.Cli.Components;

namespace AppInsights.Cli.Screens;

public static class DatabaseSetupPrompt
{
    public static bool ConfirmInitialLoad()
    {
        var choice = ChoiceDialog.Show(
            "No Lap Data Found",
            "Load the local database with race data from laptimes.json?",
            "Yes", "No");

        return choice == 0;
    }
}
