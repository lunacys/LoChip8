using System;

namespace LoChip8.Tests
{
    public class ConsoleBeeper : IBeeper
    {
        public void Beep()
        {
            Console.Beep();
        }
    }
}