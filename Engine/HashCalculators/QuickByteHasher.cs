using System;
using System.IO;
using Engine.Entities;
using Engine.Infrastructure;

namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates file hash by returning several bytes from middle of the file. 
    /// </summary>
    internal class QuickByteHasher : IHashCalculator
    {
        private readonly int sampleSize;
        private readonly long skipSize;
        private readonly ILogger logger;

        /// <param name="sampleSize">Max number of bytes to read from file. Will be less if file is shorter. Default 8.</param>
        /// <param name="skipSize">File size in bytes. Files up to this size will be skipped (not opened, hash returns empty string). Default is 0 (nothing skipped).</param>
        /// <remarks>In some cases it may be more efficient to not "peek" into small files that can be loaded in single read (mft/one cluster),
        /// and just calculate full crc in next step.</remarks>
        public QuickByteHasher(int sampleSize = 8, long skipSize = 0, ILogger logger = null)   
        {
            this.sampleSize = sampleSize;
            this.skipSize = skipSize;
            this.logger = logger ?? new NullLogger();
        }

        public byte[] ComputeHash(Duplicate duplicate)
        {
            if (duplicate.Size <= this.skipSize)
            {
                return new byte[this.sampleSize];    // Do not check the file, return empty hash.
            }

            var seekingPosition = this.CalculateSeekingPosition(duplicate.Size);
            var buffer = new byte[this.sampleSize];

            try
            {
                using (var stream = new FileStream(duplicate.FullName, FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(seekingPosition, SeekOrigin.Begin);
                    var read = 0;
                    do
                    {
                        read += stream.Read(buffer, read, this.sampleSize);
                    }
                    while (read < this.sampleSize && read < duplicate.Size);

                    return buffer;
                }
            }
            catch(Exception e)
            {
                logger.Warning($"QuickByte: Error accessing file {duplicate.FullName}. {e.Message}");
                return null; 
            }
        }

        private long CalculateSeekingPosition(long size)
        {
            if (size > this.sampleSize * 2)
            {
                return size / 2;
            }
            else
            {
                return 0;
            }            
        }
    }
}
