namespace LoChip8.Logging;

public static class Log
{
    private static readonly ConsoleColor _originalColor;
    
    public static ConsoleColor ColorDebug { get; set; } = ConsoleColor.Gray;
    public static ConsoleColor ColorInfo { get; set; } = ConsoleColor.White;
    public static ConsoleColor ColorWarning { get; set; } = ConsoleColor.Yellow;
    public static ConsoleColor ColorError { get; set; } = ConsoleColor.Red;
    public static ConsoleColor ColorFatal { get; set; } = ConsoleColor.DarkRed;

    static Log()
    {
        _originalColor = Console.ForegroundColor;
    }

    public static void Debug(object msg, string? context = null)
    {
        Console.ForegroundColor = ColorDebug;
        Console.WriteLine(Format(msg, "DEBUG", context));
        Console.ForegroundColor = _originalColor;
    }
    
    public static void Info(object msg, string? context = null)
    {
        Console.ForegroundColor = ColorInfo;
        Console.WriteLine(Format(msg, "INFO", context));
        Console.ForegroundColor = _originalColor;
    }
    
    public static void Warning(object msg, string? context = null)
    {
        Console.ForegroundColor = ColorWarning;
        Console.WriteLine(Format(msg, "WARNING", context));
        Console.ForegroundColor = _originalColor;
    }

    public static void Error(object msg, string? context = null)
    {
        Console.ForegroundColor = ColorError;
        Console.WriteLine(Format(msg, "ERROR", context));
        Console.ForegroundColor = _originalColor;
    }

    public static void Fatal(object msg, string? context = null)
    {
        Console.ForegroundColor = ColorFatal;
        Console.WriteLine(Format(msg, "FATAL", context));
        Console.ForegroundColor = _originalColor;
    }

    private static string Format(object msg, string level, string? context)
    {
        var ctxStr =  $" {context}" ?? "";

        return $"[{level}] {DateTime.UtcNow:HH:mm:ss}{ctxStr}: {msg}";
    }
}