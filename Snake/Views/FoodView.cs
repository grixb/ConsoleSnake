using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace ConsoleGame.Snake.Views;

public class FoodView : ViewBase<Food>
{
    public FoodView(Food value) : base(value)
    {
        var et = Region.EntireTerminal;
        LastSize = new(et.Width, et.Height);
    }

    public Food Food => Value;

    private Size LastSize { set; get; }

    public Snaker.Bound LastBound => (
        LastSize.Width, LastSize.Height
    );

    public override void Render(ConsoleRenderer renderer, Region? region = null)
    {
        LastSize = Measure(renderer, region);
        var cell = Food.CellWhitIn(LastBound);
        var (col, row) = (
            (region?.Left ?? 0) + cell.Col + 1,
            (region?.Top ?? 0) + cell.Row + 1);
        string to = $"{Ansi.Cursor.Move.ToLocation(col, row)}{Food.Pict}";
        renderer.RenderToRegion(to, AsNoneOverwrittenRegion(region));
    }
}