using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace AppInsights.Cli.Theming;

internal static class AppColorScheme
{
    public static ColorScheme Scheme => new()
    {
        Normal = new Attribute(AppColors.Text, AppColors.Background),
        HotNormal = new Attribute(AppColors.Text, AppColors.Background),
        Focus = new Attribute(AppColors.OnAccent, AppColors.Accent),
        HotFocus = new Attribute(AppColors.OnAccent, AppColors.Accent),
        Disabled = new Attribute(AppColors.Disabled, AppColors.Background)
    };
}
