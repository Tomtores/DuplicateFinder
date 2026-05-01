using System;
using System.IO;
using Engine.Entities;
using Engine.Infrastructure;

namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates the hash using external library and CRC32.
    /// </summary>
    internal class CRC32_Hasher : IHashCalculator
    {
        private readonly Guid? _salt;
        private readonly ILogger _logger;

        public CRC32_Hasher(Guid? salt = null, ILogger logger = null)
        {
            _salt = salt;
            _logger = logger ?? new NullLogger();
        }

        public byte[] ComputeHash(Duplicate duplicate)
        {
            try
            {
                using (var file = new FileStream(duplicate.FullName, FileMode.Open, FileAccess.Read))
                {
                    var hash = CRC32Calculator.Calculate(file, _salt);
                    return BitConverter.GetBytes(hash);
                }
            }
            catch (Exception e)
            {
                _logger.Warning($"CRC32: Failed to access file {duplicate.FullName}. {e.Message}");
                return null;
            }
        }
    }
}