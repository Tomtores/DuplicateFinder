using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Engine;
using Engine.HashCalculators;
using Engine.FileEnumerators;
using System;

namespace Test.Oldtests
{
    [TestClass]
    public class ComparisonTest
    {
        private const string TestDataPath = @"..\..\test data";
        private string[] TestDataPaths = new[] { @"..\..\test data" };
        private const string small = "small.bmp";
        private const string regular1 = "regular1.bmp";
        private const string regular2 = "regular2.bmp";
        private const string regular3 = "regular3.bmp";
        private const string regular4 = "regular4.bmp"; //this one is dupe with regular1

        [TestMethod]
        public void AreTestFilesAccessible()
        {
            var temp = Directory.GetFiles(TestDataPath, "*.*", SearchOption.AllDirectories);    //will throw if access denied
            Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, small)));
            Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, regular1)));
            Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, regular2)));
            Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, regular3)));
            Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, regular4)));
        }

        [TestMethod]
        public void GivenFakeHasher_FinderDiferentiatesBySize()
        {
            var finder = new Finder(new StandardFileEnumerator(), new[] { new FakeHasher(null) });

            finder.FindDuplicates(TestDataPaths, "*.*", new string[0]);
            var result = finder.Duplicates;

            Assert.AreEqual(1, result.Count()); //we are expecting one set of dupes
            Assert.AreEqual(4, result.Single().Count());    //with 4 items inside
        }

        [TestMethod]
        public void GivenMD5Hasher_ShouldTellFilesCorrectly()
        {
            var finder = new Finder(new StandardFileEnumerator(), new[] { new MD5_Hasher() });

            finder.FindDuplicates(TestDataPaths, "*.*", new string[0]);
            var result = finder.Duplicates;

            Assert.AreEqual(1, result.Count()); //we are expecting one set of dupes
            Assert.AreEqual(2, result.Single().Count());    //with 2 items inside
        }

        [TestMethod]
        public void GivenCRC32Hasher_ShouldTellFilesCorrectly()
        {
            var finder = new Finder(new StandardFileEnumerator(), new[] { new CRC32_Hasher(salt: Guid.NewGuid()) });

            finder.FindDuplicates(TestDataPaths, "*.*", new string[0]);
            var result = finder.Duplicates;

            Assert.AreEqual(1, result.Count()); //we are expecting one set of dupes
            Assert.AreEqual(2, result.Single().Count());    //with 2 items inside
        }

        [TestMethod]
        public void GivenQuickByteHasher_ShouldRejectFullyDifferentFile()
        {
            var finder = new Finder(new StandardFileEnumerator(), new[] { new QuickByteHasher() });

            finder.FindDuplicates(TestDataPaths, "*.*", new string[0]);
            var result = finder.Duplicates;

            Assert.AreEqual(1, result.Count()); //we are expecting one set of dupes
            Assert.AreEqual(3, result.Single().Count());    //with ~3 items inside - should reject the full red pic
        }

        [TestMethod]
        public void GivenCRCAndFakeHasher_ShouldShouldUseInSequence()
        {
            var finder = new Finder(new StandardFileEnumerator(), new IHashCalculator[] { new MD5_Hasher(), new FakeHasher(null) });

            finder.FindDuplicates(TestDataPaths, "*.*", new string[0]);
            var result = finder.Duplicates;

            Assert.AreEqual(1, result.Count()); //we are expecting one set of dupes
            Assert.AreEqual(2, result.Single().Count());
        }

        //todo
        //create two empty files, check if listed.
    }
}
