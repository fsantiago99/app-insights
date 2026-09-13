using Terminal.Gui;

namespace AppInsights.Cli.Theming;

internal static class AppColors
{
    public static Color Background { get => Terminal.Gui.Application.Force16Colors ? Color.White : field; } = new(210, 210, 210);

    public static Color Text { get => Terminal.Gui.Application.Force16Colors ? Color.Black : field; } = new(20, 20, 20);

    public static Color Disabled { get => Terminal.Gui.Application.Force16Colors ? Color.Gray : field; } = new(130, 130, 130);

    public static Color Accent { get => Terminal.Gui.Application.Force16Colors ? Color.Blue : field; } = new(0, 90, 220);

    public static Color OnAccent { get => Terminal.Gui.Application.Force16Colors ? Color.White : field; } = new(255, 255, 255);

    public static Color Yellow { get => Terminal.Gui.Application.Force16Colors ? Color.BrightYellow : field; } = new(240, 200, 0);

    public static Color Green { get => Terminal.Gui.Application.Force16Colors ? Color.Green : field; } = new(0, 170, 60);

    public static Color Purple { get => Terminal.Gui.Application.Force16Colors ? Color.Magenta : field; } = new(150, 40, 190);

    public static Color Pitlane { get => Terminal.Gui.Application.Force16Colors ? Color.Blue : field; } = new(30, 80, 200);

    public static Color Unknown { get => Terminal.Gui.Application.Force16Colors ? Color.DarkGray : field; } = new(110, 110, 110);

    public static Color NotAvailableBackground { get => Terminal.Gui.Application.Force16Colors ? Color.Black : field; } = new(30, 30, 30);

    public static Color NotAvailableForeground { get => Terminal.Gui.Application.Force16Colors ? Color.DarkGray : field; } = new(90, 90, 90);

    public static Color LogoMark { get => Terminal.Gui.Application.Force16Colors ? Color.DarkGray : field; } = new(60, 60, 60);
}
