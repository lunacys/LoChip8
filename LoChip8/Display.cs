using System;

namespace LoChip8
{
    public class Display : IDisplay
    {
        public int Width => 64;
        public int Height => 32;
        
        private byte[,] _displayData;
        public byte[,] DisplayData => _displayData;

        public Display()
        {
            _displayData = new byte[Height, Width];
            Clear();
        }
        
        public void Clear()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _displayData[i, j] = 0;
                }
            }
        }

        
        public byte DrawSprite(Sprite sprite, int positionX, int positionY)
        {
            int anyUnset = 0;
            
            var height = sprite.Height;
            for (int y = 0; y < height; y++)
            {
                var row = sprite.Rows[y];
                for (int x = 0; x < 8; x++)
                {
                    if ((row & (0x80 >> x)) != 0)
                    {
                        if (_displayData[positionY + y, positionX + x] == 1)
                            anyUnset = 1;
                        _displayData[positionY + y, positionX + x] ^= 1;
                    }
                }
            }

            return (byte) anyUnset;
        }
    }
}