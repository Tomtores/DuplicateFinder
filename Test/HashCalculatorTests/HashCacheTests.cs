using Engine.Infrastructure;
using EnginePlugins.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plugins.Cache;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Test.HashCalculatorTests
{
    [TestClass]
    public class HashCacheTests
    {
        private static (IHashCache cache, string directory) GetInstance(string folder = null)
        {
            var path = Path.Combine(Path.GetTempPath(), folder ?? "test");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return (new HashCache(path, 0L, null, new NullLogger()), path);
        }

        [TestMethod]
        public void GetItemFromEmptyCache_ReturnsNull()
        {
            // arrange
            var (cache, directory) = GetInstance();

            // act
            var result = cache.GetHash("unknown", 123, ChecksumKind.MD5, DateTime.Now);

            // assert
            Assert.IsNull(result);

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenItemAdded_ReturnsItem()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsTrue(hash.SequenceEqual(result));
            
            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenItemAddedWithDifferentHashType_ReturnsNull()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            var result = cache.GetHash(item.filename, item.size, ChecksumKind.CRC32, item.date);

            // assert
            Assert.IsNull(result);

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenMultipleItemsExist_ReturnsCorrectItem()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            var item2 = (filename: "namme2", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            var item3 = (filename: "namme3", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");

            cache.Store(item2.filename, item2.size, item2.type, Encoding.UTF8.GetBytes("hash2"), item2.date);
            cache.Store(item.filename, item.size, item.type, hash, item.date);
            cache.Store(item3.filename, item3.size, item3.type, Encoding.UTF8.GetBytes("hash3"), item3.date);

            // act
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsTrue(hash.SequenceEqual(result));

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenItemAddedThenRemoved_ReturnsNull()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            cache.Remove(item.filename);
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsNull(result);

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenItemAdded_WithResumeFromDisk_ReturnsItem()
        {
            // arrange
            var (cache, directory) = GetInstance("testFolder");
            var item = (filename: "namme", size: 567L, type: ChecksumKind.CRC32, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            cache.Flush();  //force writeout to disk
            var (cache2, directory2) = GetInstance("testFolder");
            var result = cache2.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsTrue(hash.SequenceEqual(result));

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_AfterNonExistantItemsTrimmed_ShouldreturnNull()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: Path.Combine(directory, "test1.txt"), size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            var item2 = (filename: Path.Combine(directory, "test2.txt"), size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            var item3 = (filename: Path.Combine(directory, "test3.txt"), size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");

            cache.Store(item2.filename, item2.size, item2.type, Encoding.UTF8.GetBytes("hash2"), item2.date);
            cache.Store(item.filename, item.size, item.type, hash, item.date);
            cache.Store(item3.filename, item3.size, item3.type, Encoding.UTF8.GetBytes("hash3"), item3.date);

            // act
            cache.Trim((progress) => { });
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);
            var result2 = cache.GetHash(item2.filename, item2.size, item2.type, item2.date);
            var result3 = cache.GetHash(item3.filename, item3.size, item3.type, item3.date);

            // assert
            Assert.IsNull(result);
            Assert.IsNull(result2);
            Assert.IsNull(result3);

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenRequestingItemWithChangedSize_ItemSilentlyRemoved_ReturnsNull()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            var wrongSizeRequest = cache.GetHash(item.filename, item.size + 10, item.type, item.date);
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsNull(wrongSizeRequest);
            Assert.IsNull(result);

            // cleanup
            Directory.Delete(directory, true);
        }

        [TestMethod]
        public void GetItemFromCache_WhenRequestingItemWithChangedTimestamp_ItemSilentlyRemoved_ReturnsNull()
        {
            // arrange
            var (cache, directory) = GetInstance();
            var item = (filename: "namme", size: 567L, type: ChecksumKind.MD5, date: DateTime.Now);
            byte[] hash = Encoding.UTF8.GetBytes("hash");
            cache.Store(item.filename, item.size, item.type, hash, item.date);

            // act
            var wrongDateRequest = cache.GetHash(item.filename, item.size, item.type, DateTime.Now.AddDays(5));
            var result = cache.GetHash(item.filename, item.size, item.type, item.date);

            // assert
            Assert.IsNull(wrongDateRequest);
            Assert.IsNull(result);

            // cleanup
            Directory.Delete(directory, true);
        }
    }
}
