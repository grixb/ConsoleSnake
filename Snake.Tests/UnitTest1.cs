using ConsoleGame.Snake;
using static ConsoleGame.Snake.Snaker;


using Segment = ConsoleGame.Snake.Snaker.Segment;
using Direction = ConsoleGame.Snake.Snaker.Direction;
using Position = ConsoleGame.Snake.Snaker.Position;
using Cell = ConsoleGame.Snake.Snaker.Cell;

namespace ConsoleGame.SnakeTests;

public class UnitTest1
{
    [Fact]
    public void Test_GetPositions_1()
    {
        Segment[] segments = {
            new(Direction.Right, 2),
            new(Direction.Down, 1),
            new(Direction.Right, 2),
            new(Direction.Down, 3)
        };

        var start = new Position(4, -4);

        Position[] expected = {
            new(4, -4),
            new(3, -4),
            new(2, -4),
            new(2, -3),
            new(1, -3),
            new(0, -3),
            new(0, -2),
            new(0, -1),
        };

        var actual = GetPositions(segments, start);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_GetPositions_2()
    {
        Segment[] segments = {
            new(Direction.Left, 1), new(Direction.Up, 9)
        };

        var start = new Position(-1, 1);

        Position[] expected = {
            new(-1,1),
            new(0,1),
            new(0,0),
            new(0,-1),
            new(0,-2),
            new(0,-3),
            new(0,-4),
            new(0,-5),
            new(0,-6),
            new(0,-7)
        };

        var actual = GetPositions(segments, start);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_GetCells_1()
    {
        Segment[] segments = {
            new(Direction.Up, 2),
            new(Direction.Left, 3),
            new(Direction.Up, 2),
            new(Direction.Right, 6)
        };

        Cell[] expected = {
            new(10,5),
            new(10,6),
            new(10,7),
            new(12,7),
            new(14,7),
            new(16,7),
            new(16,8),
            new(16,9),
            new(14,9),
            new(12,9),
            new(10,9),
            new(8,9),
            new(6,9)
        };

        var actual = segments
        .PositionsFrom(Position.Center)
        .CellsWithIn((20, 10))
        .ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Test_Snake_1()
    {
        var snake = new Snaker();

        snake.Snaking();
        snake.Snaking(Direction.Left);
        snake.Snaking();
        snake.Grow();
        snake.Snaking(Direction.Down);
        snake.Grow();
        snake.Snaking();

        Segment[] expected = {
            new(Direction.Down, 2),
            new(Direction.Left, 2),
            new(Direction.Up, 3)
        };

        var actual = snake.Segments.ToArray();

        Assert.Equal(expected, actual);
    }
}