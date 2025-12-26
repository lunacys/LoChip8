namespace LoChip8;

public class Display
{
    public event EventHandler? Changed; 
    
    public int Width => 64;
    public int Height => 32;

    private readonly byte[] _data;

    public byte[] Data => _data;

    public Display()
    {
        _data = new byte[Width * Height];
    }

    public void Clear()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = 0;
        }
    }

    public byte DrawSprite(Ram ram, ushort startAddress, int x, int y, int height)
    {
        byte collision = 0;

        for (int row = 0; row < height; row++)
        {
            byte spriteByte = ram.Read((ushort)(startAddress + row));
            
            for (int col = 0; col < 8; col++)
            {
                if ((spriteByte & (0x80 >> col)) != 0)
                {
                    var pixelX = (x + col) % Width;
                    var pixelY = (y + row) % Height;
                    int index = pixelY * Width + pixelX;

                    if (_data[index] == 1)
                        collision = 1;

                    _data[index] ^= 1;
                }
            }
        }
        
        Changed?.Invoke(this, EventArgs.Empty);

        return collision;
    }
}