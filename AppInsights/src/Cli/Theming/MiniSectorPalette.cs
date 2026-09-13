using Attribute = Terminal.Gui.Attribute;

namespace AppInsights.Cli.Theming;

internal static class MiniSectorPalette
{
    public static Attribute GetAttribute(char code) => code switch
    {
        '0' => new Attribute(AppColors.Text, AppColors.Yellow),
        '1' => new Attribute(AppColors.Text, AppColors.Green),
        '2' => new Attribute(AppColors.OnAccent, AppColors.Unknown),
        '3' => new Attribute(AppColors.OnAccent, AppColors.Purple),
        '4' => new Attribute(AppColors.OnAccent, AppColors.Unknown),
        '5' => new Attribute(AppColors.OnAccent, AppColors.Pitlane),
        '6' => new Attribute(AppColors.OnAccent, AppColors.Unknown),
        '7' => new Attribute(AppColors.NotAvailableForeground, AppColors.NotAvailableBackground),
        _ => new Attribute(AppColors.NotAvailableForeground, AppColors.NotAvailableBackground)
    };
}
