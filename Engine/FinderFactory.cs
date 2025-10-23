using System.Configuration;
using Engine.FileEnumerators;
using Engine.HashCalculators;
using Engine.CleanupStrategies;

namespace Engine
{
    public class FinderFactory
    {
        #region Finder

        public static IFinder CreateInstance()
        {
            return new Finder(new SafeFileEnumerator(), GetDefaultHashers());
        }

        /// <param name="enumerator">Use the Get*Enumerator to create approrpiate instance. Client may provide it's own implementation.</param>
        /// <param name="hashCalculators">A list of IHashCalculators in order they should be used. Use Get*Hasher to create configurable hashers.
        /// Files identified as identical are passed to next calculator until differences are found or no calculators are left to try.
        /// It is advised to use lightweight calculator in front and detailed one as last.</param>
        /// <returns></returns>
        public static IFinder CreateInstance(IFileEnumerator enumerator, params IHashCalculator[] hashCalculators)
        {
            if (enumerator == null)
            {
                throw new ConfigurationErrorsException("Enumerator is required!");
            }

            //we're not checking for hash calculators - given zero, we will just compare files by size. It's not recommended, but allowed.
            return new Finder(enumerator, hashCalculators);
        } 

        #endregion

        #region IFileEnumerator

        public static IFileEnumerator GetStandardFileEnumerator()
        {
            return new StandardFileEnumerator();
        }

        public static IFileEnumerator GetSafeFileEnumerator()
        {
            return new SafeFileEnumerator();
        }

        #endregion

        #region IHashCalculator

        /// <summary>
        /// Default are QuickByte followed by MD_5
        /// </summary>
        /// <returns></returns>
        public static IHashCalculator[] GetDefaultHashers()
        {
            return new IHashCalculator[] { new QuickByteHasher(), new MD5_Hasher() };
        }

        /// <param name="sampleSize">How many bytes to read for the footprint.</param>
        /// <param name="skipSize">Files under this size will be skipped with hash = "". Size in bytes.</param>
        public static IHashCalculator GetQuickByteHasher(int sampleSize = 64, int skipSize = 0)
        {
            return new QuickByteHasher(sampleSize, skipSize);
        }

        /// <param name="md5AlgorithName">Name of standard .NET MD5 algorithm name to use. Null for default implementation.</param>
        public static IHashCalculator GetMD5Hasher(string md5AlgorithName = null)
        {
            return new MD5_Hasher(md5AlgorithName);
        }

        public static IHashCalculator GetCRC32Hasher()
        {
            return new CRC32_Hasher();
        }

        #endregion  
        
        #region DeduplicationStrategies

        public static IDeduplicationStrategy GetFolderDeduplicatorStrategy()
        {
            return new RemoveExtraCopiesWithinFolderStrategy();
        }

        #endregion
    }
}
