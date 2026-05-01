using Engine.Cache;
using Engine.CleanupStrategies;
using Engine.FileEnumerators;
using Engine.HashCalculators;
using Engine.Infrastructure;
using EnginePlugins.Cache;
using Plugins;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Engine
{
    public class FinderFactory
    {
        private static Plugins.Cache.IHashCache cache;

        #region Finder

        public static IFinder CreateDefaultInstance()
        {
            return new Finder(new SafeFileEnumerator(), GetDefaultHashers());
        }

        /// <param name="enumerator">Use the Get*Enumerator to create approrpiate instance. Client may provide it's own implementation.</param>
        /// <param name="hashCalculators">A list of IHashCalculators in order they should be used. Use Get*Hasher to create configurable hashers.
        /// Files identified as identical are passed to next calculator until differences are found or no calculators are left to try.
        /// It is advised to use lightweight calculator in front and detailed one as last.</param>
        /// <returns></returns>
        public static IFinder CreateInstance(IFileAccessor enumerator, IHashCalculator[] hashCalculators, ILogger logger = null)
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
        /// <param name="useCache">True to enable caching</param>
        /// <param name="fileSizeLimit">Size limit in bytes. Files smaller than this value will not be processed by quickByte hasher and will not be cached.</param>
        /// <param name="salt">Optional salt for hash calculators. Will only be used when cache is enabled.</param>
        public static IFinder CreateConfiguredInstance(bool useCrc32, bool useCache, long fileSizeLimit, Guid? salt = null, ILogger logger = null)
        {
            var enumerator = GetSafeFileEnumerator(logger);
            var quickHasher = GetQuickByteHasher(skipSize: fileSizeLimit, logger);

            if (useCache)
            {
                InitializeCache(fileSizeLimit, salt, logger);
                var secondHasher = useCrc32 ? GetCRC32Hasher(salt, logger) : GetMD5Hasher(salt: salt, logger: logger);
                var hashers = new IHashCalculator[] 
                {
                    quickHasher,
                    PluginFactory.ApplyCaching(secondHasher, useCrc32 ? ChecksumKind.CRC32 : ChecksumKind.MD5, cache)
                };
                var finder = CreateInstance(enumerator, hashers, logger);
                return new CachedFinderDecorator(finder, cache);
            }
            else
            {
                var hashers = new IHashCalculator[] { quickHasher, useCrc32 ? GetCRC32Hasher(logger: logger) : GetMD5Hasher(logger: logger) };            
                return CreateInstance(enumerator, hashers, logger);
            }
        }        

        #endregion

        #region IFileEnumerator

        public static IFileAccessor GetStandardFileEnumerator()
        {
            return new StandardFileEnumerator();
        }

        public static IFileAccessor GetSafeFileEnumerator(ILogger logger = null)
        {
            return new SafeFileEnumerator(logger);
        }

        #endregion

        #region IHashCalculator

        /// <summary>
        /// Default are QuickByte followed by MD5
        /// </summary>
        /// <returns></returns>
        public static IHashCalculator[] GetDefaultHashers()
        {
            return new IHashCalculator[] { new QuickByteHasher(), new MD5_Hasher() };
        }

        /// <param name="skipSize">Files smaller than this bytes will not be hashed.</param>
        public static IHashCalculator GetQuickByteHasher(long skipSize = 0, ILogger logger = null)
        {
            return new QuickByteHasher(skipSize: skipSize, logger: logger);
        }

        /// <param name="md5AlgorithName">Name of standard .NET MD5 algorithm name to use. Null for default implementation.</param>
        public static IHashCalculator GetMD5Hasher(string md5AlgorithName = null, Guid? salt = null, ILogger logger = null)
        {
            return new MD5_Hasher(md5AlgorithName, salt, logger);
        }

        public static IHashCalculator GetCRC32Hasher(Guid? salt = null, ILogger logger = null)
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

        private static void InitializeCache(long sizelimit, Guid? installationSalt, ILogger logger)
        {
            if (cache != null)
            {
                (cache as IDisposable)?.Dispose();
            }

            cache = PluginFactory.ConfigureCache(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), sizelimit, installationSalt, logger);
        }

        /// <summary>
        /// May throw
        /// </summary>
        public static void TrimCache(Action<int> updateProgress, Guid? installationSalt, ILogger logger = null)
        {
            if (cache == null)
            {
                InitializeCache(0L, installationSalt, logger ?? new NullLogger());
            }

            cache.Trim(updateProgress);
        }

        #endregion
    }
}
