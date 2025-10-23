using System;
using System.IO;
using Engine.Entities;

namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates the hash using external library and CRC32.
    /// </summary>
    internal class CRC32_Hasher : IHashCalculator
    {
        public string ComputeHash(Duplicate duplicate)
        {
            try
            {
                using (var file = new FileStream(duplicate.FullName, FileMode.Open, FileAccess.Read))
                {
                    return CRC32Calculator.Calculate(file).ToString();
                }
            }
            catch (Exception)
            {
                return duplicate.FullName;  //return file path as hash, this is unique and will cause duplicate to be ruled-out from further flow.
            }
        }
    }
}