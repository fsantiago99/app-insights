using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace AppInsights.Cli.Components;

internal sealed class RichTextLine : View
{
    private readonly (string Text, Attribute? Attribute)[] _segments;

    public RichTextLine(params (string Text, Attribute? Attribute)[] segments)
    {
        _segments = segments;
        Height = 1;
        Width = segments.Sum(segment => segment.Text.Length);
    }

    protected override bool OnDrawingContent(DrawContext? context)
    {
        var x = 0;
        foreach (var (text, attribute) in _segments)
        {
            SetAttribute(attribute ?? GetNormalColor());
            Move(x, 0);
            AddStr(text);
            x += text.Length;
        }

        return true;
    }
}
