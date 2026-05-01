using System;
using System.IO;

namespace Engine.HashCalculators
{
    /// <summary>
    /// CRC32 implementation based on zlib(?)
    /// Name   : "CRC-32"
    /// Width  : 32
    /// Poly   : 04C11DB7
    /// Init   : FFFFFFFF
    /// RefIn  : True
    /// RefOut : True
    /// XorOut : FFFFFFFF
    /// Check  : CBF43926
    /// </summary>
    /// <remarks>
    /// Calculates on "reversed" bytes without actually flipping them - instead, the table is flipped by using reversed poly.
    /// </remarks>
    internal class CRC32Calculator
    {
        private const uint CRC_POLY = 0xEDB88320;   //reversed 0x04C11DB7
        private static readonly uint[] crcTable;

        static CRC32Calculator()
        {
            crcTable = CalculateCRCTable(CRC_POLY);
        }

        private static uint[] CalculateCRCTable(uint poly)
        {
            var table = new uint[256];
            for (uint row = 0; row < 256; row++)
            {
                var value = row;
                for (int bit = 0; bit < 8; bit++)
                {
                    value = (value & 1) == 1 ? poly ^ (value >> 1) : value >> 1;
                }
                table[row] = value;
            }

            return table;
        }

        public static uint Calculate(Stream file, Guid? salt)
        {
            var crc32 = 0xffffffff;  // complement it for good start
            if (salt != null)
            {
                crc32 = (uint)salt.Value.GetHashCode(); // initialize start vector with salt to prevent precomputed hash comparisons
            }

            int data;
            while ((data = file.ReadByte()) != -1)
            {
                crc32 = crcTable[(byte)(crc32 ^ data)] ^ (crc32 >> 8);
                // a lot of magic happens here:
                // first, we add (xor) the new byte into the crc and cast the result to byte to get index for table
                // then we take precalculated value from the table
                // finally we shift our crc register 8 bits right at once and xor in the result
            }

            return ~crc32;  // result xored (complement)
        }
    }
}