using System;
using System.Collections.Generic;
using System.Linq;

namespace LoChip8
{
    public class Sprite
    {
        public readonly byte[] Rows;
        public byte Height { get; }

        public Sprite(IEnumerable<byte> rows, byte height = 5)
        {
            Height = height;
            
            Rows = new byte[Height];
            
            var enumerable = rows as byte[] ?? rows.ToArray();
            for (int i = 0; i < enumerable.Length; i++)
            {
                Rows[i] = enumerable[i];
            }
        }

        public static Sprite GenerateSpriteFor(byte number)
        {
            switch (number)
            {
                case 0x0: 
                return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10010000,
                        0b10010000,
                        0b10010000,
                        0b11110000
                    });
                case 0x1:
                    return new Sprite(new byte[]
                    {
                        0b00100000,
                        0b01100000,
                        0b00100000,
                        0b00100000,
                        0b01110000
                    });
                case 0x2:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b00010000,
                        0b11110000,
                        0b10000000,
                        0b11110000
                    });
                case 0x3:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b00010000,
                        0b11110000,
                        0b00010000,
                        0b11110000
                    });
                case 0x4:
                    return new Sprite(new byte[]
                    {
                        0b10010000,
                        0b10010000,
                        0b11110000,
                        0b00010000,
                        0b00010000
                    });
                case 0x5:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10000000,
                        0b11110000,
                        0b00010000,
                        0b11110000
                    });
                case 0x6:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10000000,
                        0b11110000,
                        0b10010000,
                        0b11110000
                    });
                case 0x7:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b00010000,
                        0b00100000,
                        0b01000000,
                        0b01000000
                    });
                case 0x8:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10010000,
                        0b11110000,
                        0b10010000,
                        0b11110000
                    });
                case 0x9:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10010000,
                        0b11110000,
                        0b00010000,
                        0b11110000
                    });
                case 0xA:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10010000,
                        0b11110000,
                        0b10010000,
                        0b10010000
                    });
                case 0xB:
                    return new Sprite(new byte[]
                    {
                        0b11100000,
                        0b10010000,
                        0b11100000,
                        0b10010000,
                        0b11100000
                    });
                case 0xC:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10000000,
                        0b10000000,
                        0b10000000,
                        0b11110000
                    });
                case 0xD:
                    return new Sprite(new byte[]
                    {
                        0b11100000,
                        0b10010000,
                        0b10010000,
                        0b10010000,
                        0b11100000
                    });
                case 0xE:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10000000,
                        0b11110000,
                        0b10000000,
                        0b11110000
                    });
                case 0xF:
                    return new Sprite(new byte[]
                    {
                        0b11110000,
                        0b10000000,
                        0b11110000,
                        0b10000000,
                        0b10000000
                    });
                default:
                    throw new ArgumentOutOfRangeException(nameof(number));
            }
            
            
        }

        public static List<Sprite> GenerateDefaultSprites()
        {
            List<Sprite> sprites = new List<Sprite>();

            for (int i = 0; i < 16; i++)
            {
                sprites.Add(GenerateSpriteFor((byte)i));
            }

            return sprites;
        }
    }
}