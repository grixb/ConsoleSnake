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
        var screen = InitScreen.With(new()
        {
            Snaker = snake,
            Food = food,
            Terminal = term
        });

        using (term.WithAltScreen())
        {
            Console.Title = "Snake in Term";

            screen.Render();

            bool stopLoop = false;
            do
            {
                var keyInfo = await ReadKey(200);
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

                if (food.IsEatenBy(snake))
                {
                    snake.Grow();
                    food.TakeSomewhere();
                }

                stopLoop |= snake.IsCollide();

            } while (!stopLoop);
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

    public static UsingAltScreen WithAltScreen(this ITerminal term) =>
        new(term);

    public class UsingAltScreen : IDisposable
    {
        public ITerminal Terminal { get; init; }

        public UsingAltScreen(ITerminal term)
        {
            Terminal = term;
            Terminal.EnterAltScreen();
            Terminal.HideCursor();
            Terminal.Clear();
        }

#pragma warning disable CA1816
        public void Dispose()
        {
            Terminal.ShowCursor();
            Terminal.LeaveAltScreen();
        }
#pragma warning restore CA1816
    }
}

internal static class AnsiAux
{
    public static AnsiControlCode AltScreenOn { get; } = $"{Ansi.Esc}[?1049h";
    public static AnsiControlCode AltScreenOff { get; } = $"{Ansi.Esc}[?1049l";
}

internal readonly struct InitScreen
{
    public required ITerminal Terminal { get; init; }
    public required Snaker Snaker { get; init; }
    public required Food Food { get; init; }

    public static ScreenView With(InitScreen init) =>
        new(new(
            init.Terminal, mode: OutputMode.Ansi,
            resetAfterRender: false
            ), init.Terminal as ITerminal)
        {
            Child = new StackLayoutView
            {
                StatusLine.AsView(init.Snaker, init.Food),
                new OverlayView
                {
                    init.Snaker.GetView(),
                    init.Food.GetView()
                }
            }
        };
}
