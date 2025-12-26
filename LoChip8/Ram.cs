namespace LoChip8;

public class Ram
{
    public const ushort LoadingAddress = 0x200;
    public const ushort Size = 4096;

    public static readonly ushort MaxRomSize = Size - LoadingAddress;
    
    private readonly byte[] _data = new byte[Size];

    public ushort AddressBegin => 0x0000;
    public ushort AddressEnd => Size;
    
    private static readonly byte[] DefaultSprites = [
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    ];

    public Ram()
    {
        LoadDefaultSprites();
    }

    public void Reset()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = 0;
        }
        LoadDefaultSprites();
    }

    public void Write(ushort address, byte value)
    {
        _data[address] = value;
    }

    public byte Read(ushort address)
    {
        return _data[address];
    }

    public void LoadRom(string filename)
    {
        var bytes = File.ReadAllBytes(filename);
        LoadRom(bytes);
    }

    public void LoadRom(byte[] data)
    {
        if (data.Length >= MaxRomSize)
        {
            throw new Exception($"ROM size is too large. Max: {MaxRomSize}, got {data.Length}");
        }

        for (int i = 0; i < data.Length; i++)
        {
            _data[(ushort)(LoadingAddress + i)] = data[i];
        }
    }

    public ushort GetDefaultSpriteAddress(byte symbol)
    {
        // As the sprite data is stored starting form 0x0000 and it has size of 5 bytes,
        // return symbol * 5 (0xF - 15, address is 15 * 5 = 75 or 0x4B)
        return (ushort)(symbol * 5);
    }

    private void LoadDefaultSprites()
    {
        for (int i = 0; i < DefaultSprites.Length; i++)
        {
            _data[i] = DefaultSprites[i];
        }
    }
}