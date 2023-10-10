using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

using ConsoleGame.Snake.Views;

namespace ConsoleGame.Snake;

public static class Program
{
    public static async Task<int> Main(string[] arg)
    {
        var cons = new SystemConsole();
        if (cons.GetTerminal() is ITerminal term) { }
        else
        {
            cons.Error.WriteLine("console is not a terminal");
            return -1;
        }

        var snake = new Snaker();
        var food = new Food();
        var foodView = food.GetView();

        var screen = new ScreenView(new(
            term, mode: OutputMode.Ansi,
            resetAfterRender: false
            ), term)
        {
            Child = new StackLayoutView
            {
                StatusLine.AsView(snake, food, foodView.LastBound),
                new OverlayView
                {
                    snake.GetView(),
                    foodView
                }
            }
        };

        try
        {
            term.EnterAltScreen();
            term.HideCursor();
            term.Clear();
            Console.Title = "Snake in Term";

            screen.Render();

            bool stopLoop = false;
            do
            {
                var keyInfo = await ReadKey(250);
                term.Clear();

                if (keyInfo is ConsoleKeyInfo ki)
                {
                    snake.Snaking(
                        ki.Key switch
                        {
                            ConsoleKey.UpArrow => Snaker.Direction.Up,
                            ConsoleKey.DownArrow => Snaker.Direction.Down,
                            ConsoleKey.LeftArrow => Snaker.Direction.Left,
                            ConsoleKey.RightArrow => Snaker.Direction.Right,
                            _ => null
                        }
                    );

                    stopLoop = ki.KeyChar == 'q';
                }
                else
                    snake.Snaking();

                if (food.IsEatenBy(snake, foodView.LastBound))
                {
                    snake.Grow();
                    food.TakeSomewhere();
                }

                stopLoop |= snake.IsCollide();

            } while (!stopLoop);
        }
        finally
        {
            term.ShowCursor();
            term.LeaveAltScreen();
        }

        return 0;
    }

    public static async Task<ConsoleKeyInfo?> ReadKey(
        int msecTimeOut, bool intercept = true
    )
    {
        if (!Console.KeyAvailable)
            await Task.Delay(msecTimeOut);

        if (Console.KeyAvailable)
            return Console.ReadKey(intercept);
        else
            return null;

    }
}

internal static class ITerminalExtensions
{
    public static void SaveCursorPosition(this ITerminal term) =>
        term.Write(Ansi.Cursor.SavePosition.EscapeSequence);
    public static void RestoreCursorPosition(this ITerminal term) =>
        term.Write(Ansi.Cursor.RestorePosition.EscapeSequence);
    public static void EnterAltScreen(this ITerminal term) =>
        term.Write(AnsiAux.AltScreenOn.EscapeSequence);
    public static void LeaveAltScreen(this ITerminal term) =>
        term.Write(AnsiAux.AltScreenOff.EscapeSequence);
}

internal static class AnsiAux
{
    public static AnsiControlCode AltScreenOn { get; } = $"{Ansi.Esc}[?1049h";
    public static AnsiControlCode AltScreenOff { get; } = $"{Ansi.Esc}[?1049l";
}
