using System.CommandLine.Rendering.Views;

namespace ConsoleGame.Snake;

public class StatusLine : IObservable<string>, IDisposable
{
    public static ContentView AsView(Snaker snake, Food food, Snaker.Bound bound) =>
        ContentView.FromObservable(new StatusLine(snake, food, bound));

    private Snaker Snake { get; }
    private Food Food { get; }
    private Snaker.Bound Bound { get; }
    private List<IObserver<string>> Observers { get; } = new();

    public StatusLine(Snaker snake, Food food, Snaker.Bound bound)
    {
        (Snake, Food, Bound) = (snake, food, bound);
        Snake.Updated += StatusChanged;
        Food.Updated += StatusChanged;
    }

    private void StatusChanged(object? o, EventArgs args)
    {
        var swr = new StringWriter();
        swr.Write($"Food: {{ Pos: {Food.Position} ");
        swr.Write($"Cell: {Food.Position.ToCellWithIn(Bound)} }} | ");
        swr.Write($"Snake: {{ Head: {Snake.Head}; {Snake.Head.ToCellWithIn(Bound)} Segs: < ");
        foreach (var seg in Snake.Segments)
            swr.Write($"{seg}; ");
        swr.Write("> }}");

        foreach (var observer in Observers)
            observer.OnNext(swr.ToString());
    }

    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (!Observers.Contains(observer))
            Observers.Add(observer);
        return new UnSubscriber(Observers, observer);
    }

    public void Dispose()
    {
        foreach (var observer in Observers)
            observer.OnCompleted();
    }

    private class UnSubscriber : IDisposable
    {
        private readonly List<IObserver<string>> _observers;
        private readonly IObserver<string> _observer;

        public UnSubscriber(
            List<IObserver<string>> observers,
            IObserver<string> observer
        ) => (_observers, _observer) = (observers, observer);
        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}