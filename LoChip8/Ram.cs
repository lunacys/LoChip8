namespace LoChip8
{
    public class Ram
    {
        public static ushort LoadingAddress => 0x200;
        public static ushort Size => 4096;
        
        private byte[] _data = new byte[Size];
        
        public ushort AddressBegin => 0x0000;
        public ushort AddressEnd => Size;

        public byte this[ushort address]
        {
            get => Read(address);
            set => Write(address, value);
        }
        
        public Ram()
        {
            
        }

        public void Reset()
        {
            for (var i = 0; i < _data.Length; i++)
                _data[i] = 0;
            
            var defaultSprites = Sprite.GenerateDefaultSprites();
            
            // Store default sprites in RAM
            for (int i = 0; i < defaultSprites.Count; i++)
            {
                var sprite = defaultSprites[i];

                for (int j = 0; j < sprite.Height; j++)
                {
                    _data[i * sprite.Height + j] = sprite.Rows[j];
                }
            }
        }

        public void Write(ushort address, byte data)
        {
            _data[address] = data;
        }

        public byte Read(ushort address)
        {
            return _data[address];
        }
        
        public ushort GetSymbolAddress(byte symbol)
        {
            // As the sprite data is stored starting form 0x0000 and it has size of 5 bytes,
            // return symbol * 5 (0xF - 15, address is 15 * 5 = 75 or 0x4B)
            return (ushort) (symbol * 5);
        }
    }
}