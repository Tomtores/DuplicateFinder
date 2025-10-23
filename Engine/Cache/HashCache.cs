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

        public string GetHash(string fullName, long size, ChecksumKind kind)
        {
            HashInfo value;
            if (cache.TryGetValue(fullName, out value))
            {
                if (value.Size != size)
                {
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

        public void Store(string fullName, long size, ChecksumKind hashName, string hash)
        {
            HashInfo value;
            if (!cache.TryGetValue(fullName, out value))
            {
                value = new HashInfo { Size = size };
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
        }

        private void LoadCache()
        {
            var results = new Dictionary<string, HashInfo>();
            if (File.Exists(this.path))
            {
                using (var reader = new StreamReader(File.OpenRead(this.path), Encoding.UTF8))
                {
                    var header = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine();
                        var values = row.Split('\t');
                        results[values.GetValueSafe(0)] =
                            new HashInfo
                            {
                                Size = Convert.ToInt64(values.GetValueSafe(1)),
                                CRC32 = values.GetValueSafe(2),
                                MD5 = values.GetValueSafe(3),
                                QuickByte = values.GetValueSafe(4)
                            };
                    }
                }
            }

            this.cache = results;
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
                    file.WriteLine("Path\tSize\tCRC32\tMD5\tQuickByte\n");
                    foreach (var item in this.cache)
                    {
                        file.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", item.Key, item.Value.Size, item.Value.CRC32, item.Value.MD5, item.Value.QuickByte);
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