using Engine.Entities;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates file hash using MD5.
    /// </summary>
    internal class MD5_Hasher : IHashCalculator
    {
        public readonly string algorithm;

        public MD5_Hasher(string md5AlgorithName = null)
        {
            this.algorithm = md5AlgorithName;
        }

        public string ComputeHash(Duplicate duplicate)
        {
            try
            {
                using (var file = new FileStream(duplicate.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var md5 = string.IsNullOrWhiteSpace(this.algorithm) ? MD5.Create() : MD5.Create(this.algorithm))
                    {
                        return Convert.ToBase64String(md5.ComputeHash(file));
                    }
                }
            }
            catch (Exception)
            {
                return duplicate.FullName;  //return file path as hash, this should be unique and cause duplicate to be ruled-out from further flow.
            }
        }
    }
}