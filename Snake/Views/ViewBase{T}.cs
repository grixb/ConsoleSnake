using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace ConsoleGame.Snake.Views;

public abstract class ViewBase<T> : View, IDisposable
where T : IUpdatable
{
    public T Value { get; }

    protected ViewBase(T value)
    {
        Value = value;
        Value.Updated += ViewUpdated;
    }

    public void Dispose() =>
        Value.Updated -= ViewUpdated;

    public override Size Measure(ConsoleRenderer renderer, Size? maxSize)
    {
        var et = Region.EntireTerminal;
        return maxSize is Size m
            ? new(Math.Min(et.Width, m.Width),
                  Math.Min(et.Height, m.Height))
            : new(et.Width, et.Height);
    }

    private void ViewUpdated(object? o, EventArgs args) =>
        OnUpdated();

    protected Size Measure(ConsoleRenderer renderer, Region? region) =>
        Measure(
            renderer, (region ?? renderer.GetRegion()) is Region r 
            ? new Size(r.Width, r.Height) : null);

    protected static Region? AsNoneOverwrittenRegion(Region? region) =>
        region is Region rg ? new Region(
                rg.Left, rg.Top, rg.Width, rg.Height,
                isOverwrittenOnRender: false
            ) : region;
}
