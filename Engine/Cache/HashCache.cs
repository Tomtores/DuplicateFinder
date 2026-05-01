using Engine;
using Engine.Infrastructure;
using EnginePlugins.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Plugins.Cache
{
    internal class HashCache : IHashCache, IDisposable
    {
        private const string Version = "V3_";
        private readonly string path;
        private readonly ILogger logger;
        private readonly long sizeLimit;
        private readonly string saltChecksum;

        IDictionary<string, HashInfo> cache;
        private bool isWrittenOutToDisk = false;
        private DateTime lastSaveTime = DateTime.MinValue;
        
        private class HashInfo
        {
            public long Size;
            public byte[] CRC32; //4 bytes
            public byte[] MD5; // 16 bytes
            public DateTime fileModifiedTimestamp;
        }

        /// <summary>
        /// Initializes cache in given folder. Will skip storing files smaller than sizelimit.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public HashCache(string path, long sizelimit = 1024 * 4, Guid? installationSalt = null, ILogger logger = null)
        {
            this.logger = logger ?? new NullLogger();
            this.sizeLimit = sizelimit;
            this.saltChecksum = installationSalt == null ? string.Empty : installationSalt?.GetHashCode().ToString("X");

            if (!Directory.Exists(path))
            {
                logger.Error("Cache file folder is not accessible.");
                throw new DirectoryNotFoundException("Invalid cache location");
            }
            this.path = Path.Combine(path, "cache.tsv");
            ReadCache();
        }

        /// <summary>
        /// Returns hash for specified filepath and checksum kind. 
        /// If file size or timestamp is different than stored, file is assumed to have changed and cache entry is automatically invalidated.
        /// </summary>
        public byte[] GetHash(string fullName, long size, ChecksumKind kind, DateTime timestamp)
        {
            HashInfo value;
            if (cache.TryGetValue(fullName, out value))
            {
                if (value.Size != size || value.fileModifiedTimestamp != timestamp)
                {
                    logger.Warning($"File was modified since last access, dropping existing cache entry. File {fullName}");
                    cache.Remove(fullName); // cached item is no longer valid, remove
                    return null;
                }

                switch (kind)
                {
                    case ChecksumKind.CRC32:
                        return value.CRC32;
                    case ChecksumKind.MD5:
                        return value.MD5;
                    case ChecksumKind.QuickByte:    // we do not store quickbyte - it has zero compute cost
                    default:
                        return null;
                }
            }

            return null;
        }

        public void Store(string fullName, long size, ChecksumKind hashName, byte[] hash, DateTime fileModifiedDateUtc)
        {
            if (size < sizeLimit || hashName == ChecksumKind.QuickByte)
            {
                return; // we do not cache files smaller than size limit, we do not store quickbyte hashes
            }

            HashInfo value;
            if (!cache.TryGetValue(fullName, out value))
            {
                value = new HashInfo
                {
                    Size = size,
                    fileModifiedTimestamp = fileModifiedDateUtc
                };
                this.cache.Add(fullName, value);
            }

            switch (hashName)
            {
                case ChecksumKind.CRC32:
                    value.CRC32 = hash;
                    break;
                case ChecksumKind.MD5:
                    value.MD5 = hash;
                    break;
                default:
                    break;
            }

            value.fileModifiedTimestamp = fileModifiedDateUtc;

            isWrittenOutToDisk = false;
            PeriodicSave();
        }

        public void Remove(string fullName)
        {
            this.cache.Remove(fullName);
            this.isWrittenOutToDisk = false;
        }
        
        /// <summary>
        /// May throw
        /// </summary>
        public void Flush()
        {
            logger.Info("Cache is being flushed to disk");
            SaveCache();
        }

        public void Dispose()
        {
            try
            {
                SaveCache();
            }
            catch(Exception e)
            {
                logger.Error($"Exception when disposing: {e.Message}");
            }
        }

        private void PeriodicSave()
        {
            var timeSinceLastSave = DateTime.Now - lastSaveTime;
            if (timeSinceLastSave.TotalMinutes > 5)
            {
                try
                {
                    SaveCache();
                }
                catch(Exception e)
                {
                    logger.Error($"Exception when saving cache: {e.Message}");
                }

                lastSaveTime = DateTime.Now;
            }
        }     

        #region Cache persistence as file

        /// <summary>
        /// May throw
        /// </summary>
        private void SaveCache()
        {
            if (isWrittenOutToDisk)
            {
                return;
            }

            try
            {
                var tempPath = this.path + ".tmp";
                SaveCacheToFile(tempPath, this.cache, saltChecksum);

                if (File.Exists(this.path))
                {
                    File.Replace(tempPath, this.path, null);
                }
                else
                {
                    File.Move(tempPath, this.path);
                }

                isWrittenOutToDisk = true;
            }
            catch (Exception e)
            {
                logger.Error($"Error when writing out cache: {e.Message}");
                throw new Exception("Error when writing out cache", e);
            }
        }

        private static void SaveCacheToFile(string path, IDictionary<string, HashInfo> cache, string saltChecksum)
        {
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.WriteLine($"{Version}Path\tSize\tCRC32\tMD5\tTimestamp\t{saltChecksum}\n");
                foreach (var item in cache)
                {
                    file.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                        item.Key,
                        item.Value.Size,
                        ToByteString(item.Value.CRC32),
                        ToByteString(item.Value.MD5),
                        ToByteString(BitConverter.GetBytes(item.Value.fileModifiedTimestamp.ToBinary())));
                }
            }
        }

        private void ReadCache()
        {
            var results = new Dictionary<string, HashInfo>();

            try
            {
                if (File.Exists(this.path))
                {
                    using (var reader = new StreamReader(File.OpenRead(this.path), Encoding.UTF8))
                    {
                        var header = reader.ReadLine();
                        if (VerifyHeader(header)) // if version does not match, just drop the contents of cache
                        {
                            while (!reader.EndOfStream)
                            {
                                var row = reader.ReadLine();
                                var values = row.Split('\t');
                                if (values.Length < 2)  // skip empty rows
                                {
                                    continue;
                                }
                                results[values.GetValueSafe(0)] =
                                    new HashInfo
                                    {
                                        Size = Convert.ToInt64(values.GetValueSafe(1)),
                                        CRC32 = GetByteArray(values.GetValueSafe(2), 4),
                                        MD5 = GetByteArray(values.GetValueSafe(3), 16),
                                        fileModifiedTimestamp = ParseDate(values.GetValueSafe(4))
                                    };
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                logger.Error($"Error when reading cache: {e.Message}");
            }

            this.cache = results;
        }

        private bool VerifyHeader(string header)
        {
            if(!header.StartsWith(Version))
            {
                logger.Warning("Cache version mismatch. Cache will be dropped.");
                return false;
            }

            var headerParts = header.Split('\t');
            if (headerParts.GetValueSafe(5) != saltChecksum)
            {
                logger.Error("Cache salt checksum does not match installation value. This may be evidence of tampering. Cache will be dropped");
                return false;
            }

            return true;
        }

        private static string ToByteString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return Convert.ToBase64String(bytes);
        }

        private static DateTime ParseDate(string dateHexString)
        {
            if (string.IsNullOrWhiteSpace(dateHexString))
            {
                return DateTime.MinValue;
            }

            var decoded = Convert.FromBase64String(dateHexString);

            var date = DateTime.FromBinary(BitConverter.ToInt64(decoded, 0));
            return date;
        }

        private static byte[] GetByteArray(string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var decoded = Convert.FromBase64String(value);
            var targetArray = new byte[length];
            Buffer.BlockCopy(decoded, 0, targetArray, 0, decoded.Length);
            return targetArray;
        }

        #endregion

        #region Cache Trimming

        /// <summary>
        /// May throw
        /// </summary>
        public void Trim(Action<int> updateProgress)
        {
            var originalCount = this.cache.Count;
            var counter = 0;

            // cleanup empty folders first
            var folders = this.cache.Keys.GroupBy(k => Path.GetDirectoryName(k));
            foreach (var folder in folders)
            {
                if (!DirectoryExists(folder.Key))
                {
                    foreach (var item in folder)
                    {
                        this.Remove(item);
                        counter++;
                    }
                }

                updateProgress(counter * 100 / originalCount);
            }

            SaveCache();

            foreach (var item in this.cache.ToList())
            {
                if (!FileExists(item.Key))
                {
                    this.Remove(item.Key);
                }

                counter++;
                if (counter % 10000 == 0)
                {
                    updateProgress(counter * 100 / originalCount);
                    SaveCache();
                }
            }

            SaveCache();
        }

        private bool DirectoryExists(string path)
        {
            try
            {
                return Directory.Exists(path);
            }
            catch(Exception e)
            {
                logger.Warning($"Error when accessing directory ({path}): {e.Message}");
                return false;
            }
        }

        private bool FileExists(string filepath)
        {
            try
            {
                return File.Exists(filepath);
            }
            catch(Exception e)
            {
                logger.Warning($"Error when accessing file ({filepath}): {e.Message}");
                return false;
            }
        }

        #endregion
    }
}