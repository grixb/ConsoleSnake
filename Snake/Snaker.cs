using System.Diagnostics;

namespace ConsoleGame.Snake;

public interface IUpdatable
{
    public event EventHandler? Updated;
}

public class Snaker : IUpdatable
{
    public event EventHandler? Updated;

    public Bound LastRenderedBound { get; set; }

    private readonly LinkedList<Segment> _segments = new();

    public IEnumerable<Segment> Segments => _segments;
    public Position Head { get; private set; }

    public IEnumerable<Position> Positions =>
        _segments.PositionsFrom(Head);

    public Snaker(Position p, Segment s)
    {
        Head = p;
        _segments.AddFirst(s);
    }

    public Snaker() : this(
        new(0, 0),
        new(Direction.Up, 5)
    ) {}

    public void Snaking(Direction? newDir = null)
    {
        if (newDir is Direction nd &&
            _segments.First?.Value is Segment top)
            if (top.Dir != nd && top.Dir != nd.Opposite())
                _segments.AddFirst(new Segment(nd, 0));

        if (_segments.First is LinkedListNode<Segment> topNode)
        {
            Head = Head.Next(topNode.Value.Dir);
            topNode.ValueRef = topNode.Value + 1;
        }

        if (_segments.Last is LinkedListNode<Segment> lstNode)
        {
            if (lstNode.Value.Len > 1)
                lstNode.ValueRef = lstNode.Value - 1;
            else
                _segments.RemoveLast();
        }

        OnUpdated();
    }

    public bool IsCollide() =>
        Positions
        .Skip(1)
        .Any(p => p == Head);

    public void Grow()
    {
        if (_segments.Last is LinkedListNode<Segment> lstNode)
            lstNode.ValueRef = lstNode.Value + 1;
    }

    protected void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

    public enum Direction
    {
        Up, Left, Down, Right
    }

    public record struct Segment(
        Direction Dir, nuint Len
    )
    {
        public static Segment operator +(Segment seg, nuint inc) =>
            seg with { Len = seg.Len + inc };

        public static Segment operator -(Segment seg, nuint dec) =>
            seg with { Len = dec > seg.Len ? 0 : seg.Len - dec };

        public override readonly string ToString() =>
            $"{Dir.ToChar()}{Len}";
    }

    public record struct Position(
        nint X, nint Y
    )
    {
        public readonly Position Next(Direction dir) =>
        dir switch
        {
            Direction.Up => this with { Y = Y + 1 },
            Direction.Left => this with { X = X - 1 },
            Direction.Down => this with { Y = Y - 1 },
            Direction.Right => this with { X = X + 1 },
            _ => throw new UnreachableException()
        };

        public static readonly Position Center = new(0, 0);

        public override readonly string ToString() =>
            $"({X},{Y})";
    }

    public static IEnumerable<Position> GetPositions(
        IEnumerable<Segment> segments, Position start
    )
    {
        foreach (var (dir, len) in segments)
            for (var i = 0u; i < len; ++i)
            {
                yield return start;
                start = start.Next(dir.Opposite()); ;
            }
    }

    public record struct Cell(
        UInt16 Col, UInt16 Row
    )
    {
        public override readonly string ToString() =>
            $"[{Col},{Row}]";
    }

    public record struct Bound(
        UInt16 Width, UInt16 Height)
    {
        public static implicit operator Bound((int, int) b) =>
            new(
                b.Item1 < 0 ? (UInt16)0 : (UInt16)(b.Item1 % UInt16.MaxValue),
                b.Item2 < 0 ? (UInt16)0 : (UInt16)(b.Item2 % UInt16.MaxValue)
            );
    }

    public static IEnumerable<Cell> GetCells(
        IEnumerable<Position> positions, Bound bound
    ) => positions.Select(p => p.ToCellWithIn(bound));
}

public class Food : IUpdatable
{
    public Snaker.Position Position { get; private set; }
    public char Pict { get; init; } = '@';

    public Snaker.Bound LastRenderedBound { get; set; }

    public event EventHandler? Updated;

    public Food() => TakeSomewhere();

    private static readonly Random _rng = new();

    public void TakeSomewhere()
    {
        Position = new(_rng.Next(), _rng.Next());
        OnUpdated();
    }

    public bool IsEatenBy(Snaker snake) =>
        CellWhitIn(LastRenderedBound) == 
        snake.Head.ToCellWithIn(snake.LastRenderedBound);

    public Snaker.Cell CellWhitIn(Snaker.Bound bound) =>
        Position.ToCellWithIn(bound);

    protected void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);
}

public static class SnakeExtensions
{
    public static Snaker.Direction Opposite(this Snaker.Direction direction) =>
    direction switch
    {
        Snaker.Direction.Up => Snaker.Direction.Down,
        Snaker.Direction.Left => Snaker.Direction.Right,
        Snaker.Direction.Down => Snaker.Direction.Up,
        Snaker.Direction.Right => Snaker.Direction.Left,
        _ => throw new UnreachableException()
    };

    public static IEnumerable<Snaker.Position> PositionsFrom(
        this IEnumerable<Snaker.Segment> segments,
        Snaker.Position start
    ) => Snaker.GetPositions(segments, start);

    public static IEnumerable<Snaker.Cell> CellsWithIn(
        this IEnumerable<Snaker.Position> positions,
        Snaker.Bound bound
    ) => Snaker.GetCells(positions, bound);

    internal static nint Mod(nint a, nint b)
    {
        var c = a % b;
        if ((c < 0 && b > 0) || (c > 0 && b < 0)) {
            c += b;
        }
        return c;
    }

    internal static Snaker.Cell ToCellWithIn(
        this Snaker.Position p, Snaker.Bound b
    ) => new(
        (UInt16)Mod(b.Width / 2 + (p.X * 2), b.Width),
        (UInt16)Mod(b.Height / 2 - p.Y, b.Height)
    );

    public static char ToChar(this Snaker.Direction dir) => dir switch
    {
        Snaker.Direction.Up => 'U',
        Snaker.Direction.Left => 'L',
        Snaker.Direction.Down => 'D',
        Snaker.Direction.Right => 'R',
        _ => throw new UnreachableException()
    };
}