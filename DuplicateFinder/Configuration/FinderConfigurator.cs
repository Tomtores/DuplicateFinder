using Engine;
using Engine.HashCalculators;
using Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DuplicateFinder.Configuration
{
    public class FinderConfigurator
    {
        private static Plugins.Cache.IHashCache cache;

        internal static IFinder GetFinder(FinderSettings finderSettings)
        {
            var hashers = GetHashers(finderSettings);
            var enumerator = FinderFactory.GetSafeFileEnumerator();
            return FinderFactory.CreateInstance(enumerator, hashers);
        }

        private static IHashCalculator[] GetHashers(FinderSettings finderSettings)
        {
            if (finderSettings.UseHashCaching)
            {
                InitializeCache();
                return new IHashCalculator[] {
                    PluginFactory.ApplyCaching(FinderFactory.GetQuickByteHasher(), EnginePlugins.Cache.ChecksumKind.QuickByte, cache),
                    finderSettings.UseCRC
                        ? PluginFactory.ApplyCaching(FinderFactory.GetCRC32Hasher(), EnginePlugins.Cache.ChecksumKind.CRC32, cache)
                        : PluginFactory.ApplyCaching(FinderFactory.GetMD5Hasher(), EnginePlugins.Cache.ChecksumKind.MD5, cache) };
            }

            return new IHashCalculator[] { FinderFactory.GetQuickByteHasher(), finderSettings.UseCRC ? FinderFactory.GetCRC32Hasher() : FinderFactory.GetMD5Hasher() };
        }

        private static void InitializeCache()
        {
            cache = PluginFactory.ConfigureCache(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Ugly code. Flush cache when indexing done.
        /// May throw
        /// </summary>
        internal static void FlushCache()
        {
            if (cache != null)
            {
                cache.Flush();
            }
        }

        internal static void RemoveFromCache(IEnumerable<string> items)
        {
            if (cache != null)
            {
                try
                {
                    foreach (string item in items)
                    {
                        cache.Remove(item);
                    }

                    cache.Flush();
                }
                catch
                {
                    // ignore errors
                }
            }
        }

        /// <summary>
        /// May throw
        /// </summary>
        internal static void TrimCache(Action<int> updateProgress)
        {
            if (cache == null)
            {
                InitializeCache();
            }

            cache.Trim(updateProgress);
        }
    }
}
