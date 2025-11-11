using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test.MergeFoldersTests
{
    [TestClass]
    public class MergeFolderTests
    {
        private IFinder finder;
        private FakeFileAccessor filesystemProxy;

        public MergeFolderTests()
        {
            filesystemProxy = new FakeFileAccessor();
            finder = new FinderProxy(filesystemProxy, new FakeHasher(filesystemProxy));
        }

        [TestMethod]
        public void MergeOnEmptyPath_ReturnsEmpty()
        {
            finder.FindDuplicates(new string[] { }, "*.*", new string[] { });

            var result = finder.CalculateMergeIntoFolder("");
           
            Assert.AreEqual(0, result.FilesToMove.Count());
            Assert.AreEqual(0, result.FoldersToMove.Count());
            Assert.AreEqual(0, result.DuplicatesToDelete.Count());
        }

        [TestMethod]
        public void MergeTwoFoldersOneFileEach_ShouldDeleteOtherDuplicate()
        {
            // Arrange
            var files = new (string, byte[])[]
            {
                ( @"X:\Folder\File1.test", Encoding.UTF8.GetBytes("Content") ),
                ( @"X:\AnotherFolder\File2.txt", Encoding.UTF8.GetBytes("Content") )
            };
            filesystemProxy.SetupFiles(files);

            finder.FindDuplicates(new string[] { @"X:\" }, "*.*", new string[] { });

            // Act
            var result = finder.CalculateMergeIntoFolder(Path.GetDirectoryName(files.First().Item1));

            // Assert
            Assert.AreEqual(1, result.DuplicatesToDelete.Count());
            Assert.AreEqual(0, result.FoldersToMove.Count());
            Assert.AreEqual(0, result.FilesToMove.Count());
        }

        [TestMethod]
        public void MergeTwoFoldersOneDuplicateOneOtherFileEach_ShouldDeleteDuplicateAndMoveOtherFile()
        {
            // Arrange
            var files = new (string, byte[])[]
            {
                ( @"X:\Folder\File1.test", Encoding.UTF8.GetBytes("Content") ),
                ( @"X:\Folder\File3.test", Encoding.UTF8.GetBytes("UniqueContent1") ),
                ( @"X:\AnotherFolder\File2.txt", Encoding.UTF8.GetBytes("Content") ),
                ( @"X:\AnotherFolder\File4.txt", Encoding.UTF8.GetBytes("UniqueContent2") )
            };
            filesystemProxy.SetupFiles(files);

            finder.FindDuplicates(new string[] { @"X:\" }, "*.*", new string[] { });

            // Act
            var result = finder.CalculateMergeIntoFolder(Path.GetDirectoryName(files.First().Item1));

            // Assert
            Assert.AreEqual(1, result.DuplicatesToDelete.Count());
            Assert.AreEqual(0, result.FoldersToMove.Count());
            Assert.AreEqual(1, result.FilesToMove.Count());
        }

        [TestMethod]
        public void MergeTwoFoldersOneDuplicateOneOtherFileEachWithSubfolders_ShouldDeleteDuplicateAndMoveOtherFileAndSubfolder()
        {
            // Arrange
            var files = new (string, byte[])[]
            {
                ( @"X:\Folder\File1.test", Encoding.UTF8.GetBytes("Content") ),
                ( @"X:\Folder\File3.test", Encoding.UTF8.GetBytes("UniqueContent1") ),
                ( @"X:\AnotherFolder\File2.txt", Encoding.UTF8.GetBytes("Content") ),
                ( @"X:\AnotherFolder\File4.txt", Encoding.UTF8.GetBytes("UniqueContent2") ),
                ( @"X:\AnotherFolder\Subfolder\File4.txt", Encoding.UTF8.GetBytes("UniqueContent3") )
            };
            filesystemProxy.SetupFiles(files);
            
            finder.FindDuplicates(new string[] { @"X:\" }, "*.*", new string[] { });

            // Act
            var result = finder.CalculateMergeIntoFolder(Path.GetDirectoryName(files.First().Item1));

            // Assert
            Assert.AreEqual(1, result.DuplicatesToDelete.Count());
            Assert.AreEqual(1, result.FoldersToMove.Count());
            Assert.AreEqual(1, result.FilesToMove.Count());
        }

        // test where additional file has same name as existing file in target folder

        // test where multiple files have same name as target folder

        // test where subfolder with given name already exists - todo - merge or rename?
    }
}
