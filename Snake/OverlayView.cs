using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace ConsoleGame;

public class OverlayView : LayoutView<View>
{
    public override Size Measure(ConsoleRenderer renderer, Size maxSize)
    {
        int w = 0, h = 0;
        foreach (var s in this.Select(v => v.Measure(renderer, maxSize)))
        {
            if (s.Width > w) w = s.Width;
            if (s.Height > h) h = s.Height;
        }
        return new(w,h);
    }

    public override void Render(ConsoleRenderer renderer, Region? region = null)
    {
        foreach (var v in this) v.Render(renderer, region);
    }
}