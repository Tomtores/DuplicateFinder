using Engine.Entities;
using Engine.Infrastructure;
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
        private readonly Guid? salt;
        private readonly ILogger logger;

        public MD5_Hasher(Guid? salt, ILogger logger)
        {
            this.salt = salt;
            this.logger = logger;
        }

        public Checksum ComputeHash(Duplicate duplicate)
        {
            try
            {
                using (var file = new FileStream(duplicate.FullName, FileMode.Open, FileAccess.Read))   // todo abstract file access from hashers?
                {
                    using (var md5 = MD5.Create())
                    {
                        byte[] hash;
                        if (salt != null)
                        {
                            byte[] saltBytes = salt.Value.ToByteArray();
                            md5.TransformBlock(saltBytes, 0, saltBytes.Length, saltBytes, 0);

                            byte[] buffer = new byte[1 << 16];  // 65k
                            int bytesRead;
                            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                            }

                            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                            hash = md5.Hash;
                        }
                        else
                        {
                            hash = md5.ComputeHash(file);
                        }

                        return new Checksum(ChecksumKind.MD5.ToString("g"), hash);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warning($"MD5: Failed to access file {duplicate.FullName}. {e.Message}");
                return null;
            }
        }
    }
}