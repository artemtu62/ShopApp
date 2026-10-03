using System.Globalization;
using System.Text;
using Lab2.Part1;

Console.OutputEncoding = Encoding.UTF8;

// Инвариантная культура — чтобы дробные числа всегда выводились с точкой.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

// ==================== Задание 1 ====================
Console.WriteLine("=== Задание 1. Базовые интерфейсы ===");

IMovable point = new Point(1, 2);
Console.WriteLine($"Точка до перемещения: {point}");
point.Move(10, 20);
Console.WriteLine($"Точка после Move(10, 20): {point}");

var drawables = new List<IDrawable> { new Circle(5), new Rectangle(4, 6) };
Renderer.DrawAll(drawables);
Console.WriteLine();

// ==================== Задание 2 ====================
Console.WriteLine("=== Задание 2. Интерфейсы и наследование ===");

ShapePrinter.PrintShapeInfo(new Circle(5));
ShapePrinter.PrintShapeInfo(new Rectangle(4, 6));
ShapePrinter.Print3DShapeInfo(new Cube(3));
Console.WriteLine();

// ==================== Задание 3 ====================
Console.WriteLine("=== Задание 3. Сегрегация интерфейсов (ISP) ===");

Console.WriteLine("--- «Толстый» интерфейс IDevice (нарушение ISP) ---");
IDevice fatPrinter = new FatPrinter();
fatPrinter.Print("Report.docx");
Console.WriteLine("Вызов Scan() у принтера (пустой метод, ничего не происходит):");
fatPrinter.Scan("Report.docx");

IDevice fatScanner = new FatScanner();
fatScanner.Scan("Photo.jpg");
Console.WriteLine("Вызов Print() у сканера (пустой метод, ничего не происходит):");
fatScanner.Print("Photo.jpg");

Console.WriteLine("---- Разделённые интерфейсы IPrinter, IScanner, IFax ----");
IPrinter printer = new Printer();
printer.Print("Report.docx");

IScanner scanner = new Scanner();
scanner.Scan("Photo.jpg");

var multifunction = new MultifunctionDevice();
multifunction.Print("Contract.pdf");
multifunction.Scan("Contract.pdf");
multifunction.Fax("Contract.pdf");
Console.WriteLine();

// ==================== Задание 4 ====================
Console.WriteLine("=== Задание 4. Интерфейсы и полиморфизм ===");

PaymentProcessor.ProcessPayment(new CreditCard("1234567812345678"), 1500m);
PaymentProcessor.ProcessPayment(new Cash(), 300.5m);
Console.WriteLine();

Console.WriteLine("Работа с ConsoleLogger:");
Worker.DoWork(new ConsoleLogger());

const string logPath = "log.txt";
File.Delete(logPath);
Console.WriteLine($"Работа с FileLogger (файл {logPath}):");
Worker.DoWork(new FileLogger(logPath));
foreach (string line in File.ReadAllLines(logPath))
{
    Console.WriteLine($"  {line}");
}