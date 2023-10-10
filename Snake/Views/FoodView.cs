using System.CommandLine.Rendering;

namespace ConsoleGame.Snake.Views;

public class FoodView : ViewBase<Food>
{
    public FoodView(Food value) : base(value)
    {
    }

    public Food Food => Value;

    public override void Render(ConsoleRenderer renderer, Region? region = null)
    {
        var size = Measure(renderer, region);
        Food.LastRenderedBound = (size.Width, size.Height);
        var cell = Food.CellWhitIn(Food.LastRenderedBound);
        var (col, row) = (
            (region?.Left ?? 0) + cell.Col + 1,
            (region?.Top ?? 0) + cell.Row + 1);
        string to = $"{Ansi.Cursor.Move.ToLocation(col, row)}{Food.Pict}";
        renderer.RenderToRegion(to, AsNoneOverwrittenRegion(region));
    }
}