using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    class Simulator
    {
        public List<ushort> registers;
        public byte FLAG; 
        public ushort PC, DBUS, T, IVR, ADR, MDR, IR, RBUS, SBUS, ALU;
        public ushort SP = 65535;

        public byte[] memoryLocations = new byte[1 << 16];
        



        public Simulator()
        {
            registers = new List<ushort>();
            for(short i = 0; i < 16; i++)
            {
                registers.Add(0);
            }
            loadFileToMemory(@"C:\Users\popa_\Desktop\test.bin");
        }

        public void loadFileToMemory(string filePath)
        {
            var fileContent = File.ReadAllBytes(filePath);
            int i = 0;
            foreach(var octet in fileContent)
            {
                memoryLocations[i] = octet;
                i++;
            }
        }


    }
}
