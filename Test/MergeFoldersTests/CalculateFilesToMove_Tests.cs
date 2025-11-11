using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;

namespace Test.MergeFoldersTests
{
    [TestClass]
    public class CalculateFilesToMove_Tests
    {
        private string basePath = @"x:\test\";
        private string anotherPath = @"x:\otherDir\";
        private string anotherPath2 = @"x:\somedir\";
        private FakeFileAccessor fileAccessor = new FakeFileAccessor();

        [TestMethod]
        public void GivenEmptyAffectedFoldersList_ShouldReturnEmpty()
        {
            // Arrange
            var affectedFolders = Enumerable.Empty<string>();
            var folderFiles = new (string, byte[])[] { };
            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(folderFiles);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(0, filesToMove.Count());
        }

        [TestMethod]
        public void GivenAffectedFolderIsSameAstargetFolder_ShouldReturnEmpty()
        {
            // Arrange
            var affectedFolders = new[] { basePath };
            var folderFiles = new (string, byte[])[] { };
            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(folderFiles);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(0, filesToMove.Count());
        }

        [TestMethod]
        public void GivenTwoFilesInOtherFolderToMove_ShouldListTwoFileMoves()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };
            
            (string path, byte[]) file1 = (anotherPath + "file1.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "file2.txt", Encoding.UTF8.GetBytes("SecondContent"));
            
            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(file1, file2);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.IsTrue(filesToMove.Any(m => m.source == file1.path && m.destination == basePath + Path.GetFileName(file1.path)));
            Assert.IsTrue(filesToMove.Any(m => m.source == file2.path && m.destination == basePath + Path.GetFileName(file2.path)));
        }

        [TestMethod]
        public void GivenTwoFilesInOtherFolderPlusDuplicateToMove_ShouldListTwoFileMoves()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };
            
            (string path, byte[]) file1 = (anotherPath + "file1.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "doooplicate.txt", Encoding.UTF8.GetBytes("Duplicate"));
            (string path, byte[]) file3 = (anotherPath + "file2.txt", Encoding.UTF8.GetBytes("SecondContent"));
            
            var duplicatesToDelete = new [] { file2.path };

            fileAccessor.SetupFiles(file1, file2, file3);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.IsTrue(filesToMove.Any(m => m.source == file1.path && m.destination == basePath + Path.GetFileName(file1.path)));
            Assert.IsTrue(filesToMove.Any(m => m.source == file3.path && m.destination == basePath + Path.GetFileName(file3.path)));
        }

        [TestMethod]
        public void GivenTwoFilesInTwoOtherFolderToMove_ShouldListTwoFileMoves()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };
            
            (string path, byte[]) file1 = (anotherPath + "file1.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath2 + "file2.txt", Encoding.UTF8.GetBytes("SecondContent"));

            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(file1, file2);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.IsTrue(filesToMove.Any(m => m.source == file1.path && m.destination == basePath + Path.GetFileName(file1.path)));
            Assert.IsTrue(filesToMove.Any(m => m.source == file2.path && m.destination == basePath + Path.GetFileName(file2.path)));
        }

        [TestMethod]
        public void GivenFileToMoveHasSameNameAsExisting_ShouldListMoveWithRename()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string path, byte[]) file1 = (basePath + "samename.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "samename.txt", Encoding.UTF8.GetBytes("SecondContent"));

            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(file1, file2);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(1, filesToMove.Count());
            var itemToMove = filesToMove.First();
            Assert.IsTrue(itemToMove.source == file2.path);
            Assert.IsTrue(itemToMove.destination != file1.path, "File should not be overwritten!");
            Assert.IsTrue(Path.GetDirectoryName(itemToMove.destination).AddDirSeparator() == basePath);
        }

        [TestMethod]
        public void GivenFileToMoveHasSameNameAsExistingButIsDuplicate_ShouldRuleOutDuplicate()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string path, byte[]) file1 = (basePath + "file1.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "file1.txt", Encoding.UTF8.GetBytes("Duplicate"));            
            (string path, byte[]) file3 = (anotherPath + "anotherfile.txt", Encoding.UTF8.GetBytes("SecondContent"));            

            var duplicatesToDelete = new[] { file2.path };

            fileAccessor.SetupFiles(file1, file2, file3);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(1, filesToMove.Count());
            Assert.IsTrue(filesToMove.Any(m => m.source == file3.path && m.destination == basePath + Path.GetFileName(file3.path)));
        }

        [TestMethod]
        public void GivenTwoFilesToMoveHaveSameName_ShouldRenameOne()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };

            (string path, byte[]) file1 = (basePath + "file1.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "samename.txt", Encoding.UTF8.GetBytes("SecondContent"));
            (string path, byte[]) file3 = (anotherPath2 + "samename.txt", Encoding.UTF8.GetBytes("ThirdContent"));

            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(file1, file2, file3);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.AreNotEqual(filesToMove.First().destination, filesToMove.Skip(1).First().destination, "File should not be overwriten!");
            Assert.IsTrue(filesToMove.Any(m => m.source == file2.path && m.destination.StartsWith(basePath)));
            Assert.IsTrue(filesToMove.Any(m => m.source == file3.path && m.destination.StartsWith(basePath)));
        }

        [TestMethod]
        public void GivenMultipleFilesToMoveAndTargetFolderFileHaveSameName_ShouldRenameAllOthers()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };

            (string path, byte[]) file1 = (basePath + "samename.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "samename.txt", Encoding.UTF8.GetBytes("SecondContent"));
            (string path, byte[]) file3 = (anotherPath2 + "samename.txt", Encoding.UTF8.GetBytes("ThirdContent"));

            var duplicatesToDelete = Enumerable.Empty<string>();

            fileAccessor.SetupFiles(file1, file2, file3);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.AreNotEqual(filesToMove.First().destination, filesToMove.Skip(1).First().destination, "File should not be overwriten!");
            Assert.IsTrue(filesToMove.Any(m => m.source == file2.path && m.destination.StartsWith(basePath) && m.destination != file1.path));
            Assert.IsTrue(filesToMove.Any(m => m.source == file3.path && m.destination.StartsWith(basePath) && m.destination != file1.path));
        }

        [TestMethod]
        public void GivenMultipleFilesToMoveWithDuplicatesAndTargetFolderFileHaveSameName_ShouldRenameAllOthersSkipDuplicate()
        {
            // Arrange
            const string thirdPath = @"Z:\thirdpath\";
            var affectedFolders = new[] { anotherPath, anotherPath2, thirdPath };

            (string path, byte[]) file1 = (basePath + "samename.txt", Encoding.UTF8.GetBytes("Content"));
            (string path, byte[]) file2 = (anotherPath + "samename.txt", Encoding.UTF8.GetBytes("Duplicate"));
            (string path, byte[]) file3 = (anotherPath2 + "samename.txt", Encoding.UTF8.GetBytes("ThirdContent"));
            (string path, byte[]) file4 = (thirdPath + "samename.txt", Encoding.UTF8.GetBytes("FourthContent"));

            var duplicatesToDelete = new[] { file2.path };

            fileAccessor.SetupFiles(file1, file2, file3, file4);

            // Act
            var filesToMove = FinderProxy.CalculateFilesToMove_Test(basePath, affectedFolders, duplicatesToDelete, fileAccessor);

            // Assert
            Assert.AreEqual(2, filesToMove.Count());
            Assert.AreNotEqual(filesToMove.First().destination, filesToMove.Skip(1).First().destination, "File should not be overwriten!");
            Assert.IsTrue(filesToMove.Any(m => m.source == file3.path && m.destination.StartsWith(basePath) && m.destination != file1.path));
            Assert.IsTrue(filesToMove.Any(m => m.source == file4.path && m.destination.StartsWith(basePath) && m.destination != file1.path));
        }
    }
}
