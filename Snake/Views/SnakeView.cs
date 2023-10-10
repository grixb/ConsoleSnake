using System.CommandLine.Rendering;

namespace ConsoleGame.Snake.Views;

public class SnakeView : ViewBase<Snaker>
{
    public SnakeView(Snaker value) : base(value)
    {
    }

    public Snaker Snake => Value;

    public override void Render(ConsoleRenderer renderer, Region? region = null)
    {
        var size =  Measure(renderer, region);
        Snake.LastRenderedBound = (size.Width, size.Height);
        var cells = Snake.Positions
        .CellsWithIn(Snake.LastRenderedBound);

        var swr = new StringWriter();

        foreach (var c in cells)
        {
            swr.Write(Ansi.Cursor.Move.ToLocation(
                (region?.Left ?? 0) + c.Col + 1,
                (region?.Top ?? 0) + c.Row + 1));
            swr.Write("*");
        }

        renderer.RenderToRegion(
            swr.ToString(),
            AsNoneOverwrittenRegion(region));
    }
}