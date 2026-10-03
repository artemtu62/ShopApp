namespace Lab2.Part1;

public interface IShape
{
    double GetArea();
    double GetPerimeter();
}

public interface I3DShape : IShape
{
    double GetVolume();
}

public class Circle : IShape, IDrawable
{
    public double Radius { get; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public double GetArea() => Math.PI * Radius * Radius;
    public double GetPerimeter() => 2 * Math.PI * Radius;
    public void Draw() => Console.WriteLine($"Рисуем круг радиусом {Radius}");
}

public class Rectangle : IShape, IDrawable
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double GetArea() => Width * Height;
    public double GetPerimeter() => 2 * (Width + Height);
    public void Draw() => Console.WriteLine($"Рисуем прямоугольник {Width} x {Height}");
}

public class Cube : I3DShape
{
    public double Side { get; }

    public Cube(double side)
    {
        Side = side;
    }

    public double GetArea() => 6 * Side * Side;

    public double GetPerimeter() => 12 * Side;

    public double GetVolume() => Side * Side * Side;
}

public static class ShapePrinter
{
    public static void PrintShapeInfo(IShape shape)
    {
        Console.WriteLine(
            $"{shape.GetType().Name}: площадь = {shape.GetArea():F2}, периметр = {shape.GetPerimeter():F2}");
    }

    public static void Print3DShapeInfo(I3DShape shape)
    {
        PrintShapeInfo(shape);
        Console.WriteLine($"  объём = {shape.GetVolume():F2}");
    }
}