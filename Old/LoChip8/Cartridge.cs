using System.IO;

namespace LoChip8
{
    public class Cartridge
    {
        private readonly Ram _ram;
        
        public Cartridge(Ram ram)
        {
            _ram = ram;
        }
        
        public int LoadRom(string filename)
        {
            var bufferSize = Ram.Size - Ram.LoadingAddress;
            
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    var bytes = br.ReadBytes(bufferSize);
                    
                    return LoadRom(bytes);
                }
            }
        }

        public int LoadRom(byte[] romData)
        {
            for (ushort i = 0; i < romData.Length; i++)
            {
                _ram[(ushort) (Ram.LoadingAddress + i)] = romData[i];
            }

            return romData.Length;
        }
    }
}