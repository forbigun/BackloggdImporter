using System;

namespace BackloggdImporter.Helpers;

internal static class ConsolePrinter
{
    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}