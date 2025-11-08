using Engine;
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
        private string path;
        IDictionary<string, HashInfo> cache;
        private bool isWrittenOutToDisk = false;
        private DateTime lastSaveTime = DateTime.MinValue;

        public HashCache(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Invalid cache location");
            }
            this.path = Path.Combine(path, "cache.tsv");
            LoadCache();
        }

        public string GetHash(string fullName, long size, ChecksumKind kind, DateTime timestamp)
        {
            HashInfo value;
            if (cache.TryGetValue(fullName, out value))
            {
                if (value.Size != size)
                {
                    cache.Remove(fullName); // cached item is no longer valid, remove
                    return null;    
                }

                // Compatibility fixup for old cache entries without timestamp
                if (value.fileModifiedTimestamp == null)
                {
                    value.fileModifiedTimestamp = timestamp;
                }

                if (value.fileModifiedTimestamp != timestamp)
                { 
                    cache.Remove(fullName); // cached item is no longer valid, remove
                    return null;
                }

                switch (kind)
                {
                    case ChecksumKind.CRC32:
                        return value.CRC32;
                    case ChecksumKind.MD5:
                        return value.MD5;
                    case ChecksumKind.QuickByte:
                        return value.QuickByte;
                }
            }

            return null;
        }

        public void Store(string fullName, long size, ChecksumKind hashName, string hash, DateTime fileModifiedDateUtc)
        {
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
                case ChecksumKind.QuickByte:
                    value.QuickByte = hash;
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

        private void PeriodicSave()
        {
            var timeSinceLastSave = DateTime.Now - lastSaveTime;
            if (timeSinceLastSave.TotalMinutes > 5)
            {
                try
                {
                    SaveCache();
                }
                catch
                {
                    // must not fail
                }

                lastSaveTime = DateTime.Now;
            }
        }

        private class HashInfo
        {
            public long Size;
            public string CRC32;
            public string MD5;
            public string QuickByte;
            public DateTime? fileModifiedTimestamp;
        }

        private void LoadCache()
        {
            var results = new Dictionary<string, HashInfo>();
            if (File.Exists(this.path))
            {
                using (var reader = new StreamReader(File.OpenRead(this.path), Encoding.UTF8))
                {
                    var header = reader.ReadLine();
                    var timestampModeEnabled = !string.IsNullOrWhiteSpace(header.Split('\t').GetValueSafe(5));
                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine();
                        var values = row.Split('\t');
                        if (values.Length < 2)
                        {
                            continue;
                        }
                        results[values.GetValueSafe(0)] =
                            new HashInfo
                            {
                                Size = Convert.ToInt64(values.GetValueSafe(1)),
                                CRC32 = values.GetValueSafe(2),
                                MD5 = values.GetValueSafe(3),
                                QuickByte = values.GetValueSafe(4),
                                fileModifiedTimestamp = timestampModeEnabled ? ParseDate(values.GetValueSafe(5)) : null
                            };
                    }
                }
            }

            this.cache = results;
        }

        private static DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            var date = DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return date;
        }

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
                using (var file = new StreamWriter(this.path, false, Encoding.UTF8))
                {
                    file.WriteLine("Path\tSize\tCRC32\tMD5\tQuickByte\tTimestamp\n");
                    foreach (var item in this.cache)
                    {
                        file.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", item.Key, item.Value.Size, item.Value.CRC32, item.Value.MD5, item.Value.QuickByte, item.Value.fileModifiedTimestamp?.ToString("o"));
                    }
                }

                isWrittenOutToDisk = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error when writing out cache", e);
            }
        }

        public void Dispose()
        {
            try
            {
                SaveCache();
            }
            catch
            {
                // must not fail
            }
        }

        /// <summary>
        /// May throw
        /// </summary>
        public void Flush()
        {
            SaveCache();
        }

        /// <summary>
        /// May throw
        /// </summary>
        public void Trim(Action<int> updateProgress)
        {
            var originalCount = this.cache.Count;
            var counter = 0;

            foreach (var item in this.cache.ToList())
            {
                if (!FileExists(item))
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

        private bool FileExists(KeyValuePair<string, HashInfo> pair)
        {
            try
            {
                return File.Exists(pair.Key);
            }
            catch (Exception _)
            {
                return false;
            }
        }
    }
}