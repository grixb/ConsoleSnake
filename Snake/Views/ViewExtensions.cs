namespace ConsoleGame.Snake.Views;

public static class ViewExtensions
{
    public static SnakeView GetView(this Snaker snake) =>
        new (snake);

    public static FoodView GetView(this Food food) =>
        new (food);
}