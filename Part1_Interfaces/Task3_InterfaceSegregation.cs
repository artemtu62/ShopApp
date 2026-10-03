namespace Lab2.Part1;


public interface IDevice
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
}

public class FatPrinter : IDevice
{
    public void Print(string document) => Console.WriteLine($"FatPrinter: печать документа \"{document}\"");

    public void Scan(string document) { }
    public void Fax(string document) { }
}

public class FatScanner : IDevice
{
    public void Print(string document) { }
    public void Scan(string document) => Console.WriteLine($"FatScanner: сканирование документа \"{document}\"");
    public void Fax(string document) { }
}


public interface IPrinter
{
    void Print(string document);
}

public interface IScanner
{
    void Scan(string document);
}

public interface IFax
{
    void Fax(string document);
}

public class Printer : IPrinter
{
    public void Print(string document) => Console.WriteLine($"Printer: печать документа \"{document}\"");
}

public class Scanner : IScanner
{
    public void Scan(string document) => Console.WriteLine($"Scanner: сканирование документа \"{document}\"");
}

public class MultifunctionDevice : IPrinter, IScanner, IFax
{
    public void Print(string document) => Console.WriteLine($"MultifunctionDevice: печать документа \"{document}\"");
    public void Scan(string document) => Console.WriteLine($"MultifunctionDevice: сканирование документа \"{document}\"");
    public void Fax(string document) => Console.WriteLine($"MultifunctionDevice: отправка факса \"{document}\"");
}