namespace Lab2.Part1;

public interface IMovable
{
    void Move(int x, int y);
}

public class Point : IMovable
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Move(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"Point({X}, {Y})";
}

public interface IDrawable
{
    void Draw();
}

public static class Renderer
{
    public static void DrawAll(List<IDrawable> shapes)
    {
        foreach (IDrawable shape in shapes)
        {
            shape.Draw();
        }
    }
}