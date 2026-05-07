using Engine.Cache;
using Engine.CleanupStrategies;
using Engine.Entities;
using Engine.FileEnumerators;
using Engine.HashCalculators;
using Engine.Infrastructure;
using Plugins;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Engine
{
    public static class FinderFactory
    {
        private static Plugins.Cache.IHashCache cache;

        #region Finder

        public static IFinder CreateDefaultInstance()
        {
            var logger = new NullLogger();
            return new Finder(new SafeFileEnumerator(logger), GetDefaultHashers(logger), logger);
        }

        /// <param name="enumerator">Use the Get*Enumerator to create approrpiate instance. Client may provide it's own implementation.</param>
        /// <param name="hashCalculators">A list of IHashCalculators in order they should be used. Use Get*Hasher to create configurable hashers.
        /// Files identified as identical are passed to next calculator until differences are found or no calculators are left to try.
        /// It is advised to use lightweight calculator in front and detailed one as last.</param>
        /// <returns></returns>
        public static IFinder CreateInstance(IFileAccessor enumerator, IHashCalculator[] hashCalculators, ILogger logger)
        {
            if (enumerator == null)
            {
                throw new ConfigurationErrorsException("Enumerator is required!");
            }

            // we are not checking for hash calculators - given zero, we will just compare files by size. It's not recommended, but allowed.
            return new Finder(enumerator, hashCalculators, logger);
        }

        /// <summary>
        /// Configure finder instance.
        /// </summary>
        /// <param name="useCrc32">Use Crc32 instead of MD5</param>
        /// <param name="cachePath">Set path to enable caching</param>
        /// <param name="cacheEntrySizeLimit">Size limit in bytes. Hashes of files smaller than this value will not be cached. Set to 0 to cache all files.</param>
        /// <param name="fileSizeLimit">Size limit in bytes. Files smaller than this value will not be processed by quickByte hasher.</param>
        /// <param name="salt">Optional salt for hash calculators. Will only be used when cache is enabled.</param>
        public static IFinder CreateConfiguredInstance(bool useCrc32, string cachePath, long cacheEntrySizeLimit, long cacheSizeLimitForMD5, long fileSizeLimit, Guid? salt, ILogger logger)
        {
            var enumerator = GetSafeFileEnumerator(logger);
            
            if (!string.IsNullOrWhiteSpace(cachePath))
            {
                InitializeCache(cachePath, cacheEntrySizeLimit, cacheSizeLimitForMD5, salt, logger);
                var hashers = new IHashCalculator[]
                {
                    PluginFactory.ApplyCaching(GetQuickByteHasher(fileSizeLimit, logger), ChecksumKind.QuickByte, cache),
                    useCrc32
                        ? PluginFactory.ApplyCaching(GetCRC32Hasher(salt, logger), ChecksumKind.CRC32, cache)
                        : PluginFactory.ApplyCaching(GetMD5Hasher(salt, logger), ChecksumKind.MD5, cache)
                };
                var finder = CreateInstance(enumerator, hashers, logger);
                return new CachedFinderDecorator(finder, cache);
            }
            else
            {
                var hashers = new IHashCalculator[] 
                {
                    GetQuickByteHasher(fileSizeLimit, logger), 
                    useCrc32 ? GetCRC32Hasher(salt: null, logger) : GetMD5Hasher(salt: null, logger) 
                };            
                return CreateInstance(enumerator, hashers, logger);
            }
        }        

        #endregion

        #region IFileEnumerator

        public static IFileAccessor GetStandardFileEnumerator()
        {
            return new StandardFileEnumerator();
        }

        public static IFileAccessor GetSafeFileEnumerator(ILogger logger)
        {
            return new SafeFileEnumerator(logger);
        }

        #endregion

        #region IHashCalculator

        /// <summary>
        /// Default are QuickByte followed by MD5
        /// </summary>
        /// <returns></returns>
        public static IHashCalculator[] GetDefaultHashers(ILogger logger)
        {
            return new IHashCalculator[] { new QuickByteHasher(0, logger), new MD5_Hasher(null, logger) };
        }

        /// <param name="skipSize">Files smaller than this bytes will not be hashed.</param>
        public static IHashCalculator GetQuickByteHasher(long skipSize, ILogger logger)
        {
            return new QuickByteHasher(skipSize, logger);
        }

        /// <param name="salt">Provide optional salt to seed calculation or null.</param>
        public static IHashCalculator GetMD5Hasher(Guid? salt, ILogger logger)
        {
            return new MD5_Hasher(salt, logger);
        }

        public static IHashCalculator GetCRC32Hasher(Guid? salt, ILogger logger)
        {
            return new CRC32_Hasher(salt, logger);
        }

        #endregion  
        
        #region DeduplicationStrategies

        public static IDeduplicationStrategy GetFolderDeduplicatorStrategy()
        {
            return new RemoveExtraCopiesWithinFolderStrategy();
        }

        #endregion

        #region CacheManagement

        private static void InitializeCache(string cachepath, long sizelimit, long sizeLimitForMD5, Guid? installationSalt, ILogger logger)
        {
            if (cache != null)
            {
                (cache as IDisposable)?.Dispose();
            }

            cache = PluginFactory.ConfigureCache(cachepath, sizelimit, sizeLimitForMD5, installationSalt, logger);
        }

        /// <summary>
        /// May throw
        /// </summary>
        public static void TrimCache(string cachePath, Action<int> updateProgress, Guid? installationSalt, ILogger logger)
        {
            if (cache == null)
            {
                InitializeCache(cachePath, 0L, 0L, installationSalt, logger);
            }

            cache.Trim(updateProgress);
        }

        #endregion
    }
}
