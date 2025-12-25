using System;

namespace LoChip8;

public class ConsoleBeeper : IBeeper
{
    public void Beep(int duration)
    {
        Console.Beep(1000, duration);
    }
}