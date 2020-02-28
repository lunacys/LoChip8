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

        
        public bool DrawSprite(Sprite sprite, int positionX, int positionY)
        {
            for (int i = 0; i < sprite.Rows.Length; i++)
            {
                var row = sprite.Rows[i];

                /*byte dRow = 0;
                for (int j = 0; j < 8; j++) // 8 bit
                {
                    dRow |= (byte) (_displayData[positionX + j, positionY + i] ? 1 : 0);

                    if (i != 7)
                        dRow <<= 1;
                }

                var resultRow = row ^ dRow;

                bool[] rRow = new bool[8];*/
                for (int j = 0; j < Math.Min(8, Width - positionX); j++)
                {
                    _displayData[positionY + i, positionX + j] = ((row >> j) & 0b0000_0001) == 1;
                }
            }

            return false;
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