using System;

namespace LoChip8
{
    public class Display : IDisplay
    {
        public int Width => 64;
        public int Height => 32;
        
        private bool[,] _displayData;
        public bool[,] DisplayData => _displayData;

        public Display()
        {
            _displayData = new bool[Height, Width];
            Clear();
        }
        
        public void Clear()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _displayData[i, j] = false;
                }
            }
        }
        
        public override string ToString()
        {
            var result = "";

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    result += _displayData[i, j] ? '#' : '*';
                }

                result += "\n";
            }

            return result;
        }
    }
}