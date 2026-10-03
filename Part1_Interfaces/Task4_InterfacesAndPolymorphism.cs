namespace Lab2.Part1;


public interface IPayable
{
    void Pay(decimal amount);
}

public class CreditCard : IPayable
{
    private readonly string _number;

    public CreditCard(string number)
    {
        _number = number;
    }

    public void Pay(decimal amount)
    {
        string last4 = _number.Length >= 4 ? _number[^4..] : _number;
        Console.WriteLine($"Оплата картой **** {last4}: {amount:F2}");
    }
}

public class Cash : IPayable
{
    public void Pay(decimal amount) =>
        Console.WriteLine($"Оплата наличными: {amount:F2}");
}

public static class PaymentProcessor
{
    public static void ProcessPayment(IPayable method, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сумма должна быть положительной");
        }

        Console.WriteLine("Обработка платежа...");
        method.Pay(amount);
        Console.WriteLine("Платёж успешно проведён.");
    }
}


public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[Console] {message}");
}

public class FileLogger : ILogger
{
    private readonly string _path;

    public FileLogger(string path)
    {
        _path = path;
    }

    public void Log(string message) =>
        File.AppendAllText(_path, $"[File] {message}{Environment.NewLine}");
}

public static class Worker
{
    public static void DoWork(ILogger logger)
    {
        logger.Log("Начало работы");
        logger.Log("Выполняется операция...");
        logger.Log("Работа завершена");
    }
}