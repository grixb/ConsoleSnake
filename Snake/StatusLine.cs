using System.CommandLine.Rendering.Views;
using System.Text;

namespace ConsoleGame.Snake;

public class StatusLine : IObservable<string>, IDisposable
{
    public static ContentView AsView(Snaker snake, Food food) =>
        ContentView.FromObservable(new StatusLine(snake, food));

    private Snaker Snake { get; }
    private Food Food { get; }
    private List<IObserver<string>> Observers { get; } = new();

    public StatusLine(Snaker snake, Food food)
    {
        (Snake, Food) = (snake, food);
        Snake.Updated += StatusChanged;
        Food.Updated += StatusChanged;
    }

    private readonly StringBuilder swr = new();

    private void StatusChanged(object? o, EventArgs args)
    {
        swr.Clear();
        swr.Append($"Food: {{ Pos: {Food.Position} ");
        swr.Append($"Cell: {Food.Position.ToCellWithIn(Food.LastRenderedBound)} }} | ");
        swr.Append($"Snake: {{ Head: {Snake.Head}; {Snake.Head.ToCellWithIn(Snake.LastRenderedBound)} Segs: < ");
        foreach (var seg in Snake.Segments)
            swr.Append($"{seg}; ");
        swr.Append("> }}");

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