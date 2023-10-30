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

        // var swr = new StringWriter();

        // foreach (var c in cells)
        // {
        //     swr.Write(Ansi.Cursor.Move.ToLocation(
        //         (region?.Left ?? 0) + c.Col + 1,
        //         (region?.Top ?? 0) + c.Row + 1));
        //     swr.Write("*");
        // }

        // renderer.RenderToRegion(
        //     swr.ToString(),
        //     AsNoneOverwrittenRegion(region));

        foreach (var c in cells)
            renderer.RenderToRegion("*", region.MoveTo(c));
    }
}

public static class SnakeViewExtensions
{
    public static Region? MoveTo(this Region? r, Snaker.Cell c) =>
        r is not null
        ? new Region(
            r.Left + c.Col, r.Top + c.Row, 1, 1,
            isOverwrittenOnRender: false
        )
        : null;
}