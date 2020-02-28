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
            bool anyUnset = false;
            
            for (int i = 0; i < sprite.Rows.Length; i++)
            {
                var row = sprite.Rows[i];
                
                for (int j = 0; j < 8; j++)
                {
                    var pixel = (row >> (7 - j)) & 1;
                    var posXWrapped = (positionX + j) % Width;
                    var posYWrapped = (positionY + i) % Width;
                    var data = _displayData[posYWrapped, posXWrapped];

                    if (data && pixel == 0)
                        anyUnset = true;
                    
                    _displayData[posYWrapped, posXWrapped] ^= (pixel == 1);
                }
            }

            return anyUnset;
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