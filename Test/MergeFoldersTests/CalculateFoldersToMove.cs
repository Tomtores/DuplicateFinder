using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test.MergeFoldersTests
{
    [TestClass]
    public class CalculateFoldersToMove
    {
        private string basePath = @"x:\test\";
        private string anotherPath = @"x:\otherDir\";
        private string anotherPath2 = @"x:\somedir\";
        private FakeFileAccessor fileAccessor = new FakeFileAccessor();

        [TestMethod]
        public void GivenNoSubfoldersInSourceFolders_ShouldListNoMoves()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { });
            (string parent, string[] subfolders) dir1 = (anotherPath, new string[] { });

            fileAccessor.SetupDirectories(baseDir, dir1);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(0, foldersToMove.Count());           
        }

        [TestMethod]
        public void GivenOneSubfoldersInSourceFolders_ShouldListOneMove()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "subdir" });

            fileAccessor.SetupDirectories(baseDir, dir1);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(1, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.destination == (basePath + dir1.subfolders[0]).AddDirSeparator()));
        }

        [TestMethod]
        public void GivenSubFolderInTargetFolder_ShouldListNoMoves()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { "subfolder" });
            (string parent, string[] subfolders) dir1 = (anotherPath, new string[] { });

            fileAccessor.SetupDirectories(baseDir, dir1);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(0, foldersToMove.Count());
        }

        [TestMethod]
        public void GivenOneSubfolderInSourceAndOneIntarget_ShouldListOneMove()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { "some dir" });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "subdir" });

            fileAccessor.SetupDirectories(baseDir, dir1);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(1, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.destination == (basePath + dir1.subfolders[0]).AddDirSeparator()));
        }

        [TestMethod]
        public void GivenOneSubfolderInSourceAndOneIntargetWithSameName_ShouldListOneMoveWithRename()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { "some dir" });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "some dir" }); //duplicate name

            fileAccessor.SetupDirectories(baseDir, dir1);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(1, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.destination != (baseDir.parent + baseDir.subfolders[0]).AddDirSeparator()));
        }

        [TestMethod]
        public void GivenTwoSourceFolderWithSamenameSubfolders_ShouldListtwoMovesWithOneRename()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "subfolder" }); 
            (string parent, string[] subfolders) dir2 = (anotherPath2, new[] { "subfolder" }); //duplicate name

            fileAccessor.SetupDirectories(baseDir, dir1, dir2);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(2, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir2.parent + dir2.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.First().destination != foldersToMove.Skip(1).First().destination);
        }

        [TestMethod]
        public void GivenTwoSourceFoldersAndTargetHaveSameSubfolder_ShouldListtwoRenames()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { "subfolder" });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "subfolder" });   //duplicate name
            (string parent, string[] subfolders) dir2 = (anotherPath2, new[] { "subfolder" }); //duplicate name

            fileAccessor.SetupDirectories(baseDir, dir1, dir2);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(2, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir2.parent + dir2.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.First().destination != foldersToMove.Skip(1).First().destination);
            Assert.IsTrue(foldersToMove.All(f => f.destination != (baseDir.parent + baseDir.subfolders[0]).AddDirSeparator()));
        }

        [TestMethod]
        public void GivenMultipleSourceFoldersAndTargetHaveSameSubfolderWithUnusedFolders_ShouldListTwoRenames()
        {
            // Arrange
            var affectedFolders = new[] { anotherPath, anotherPath2 };

            (string parent, string[] subfolders) baseDir = (basePath, new string[] { "subfolder" });
            (string parent, string[] subfolders) dir1 = (anotherPath, new[] { "subfolder" });   //duplicate name
            (string parent, string[] subfolders) dir1a = (@"z:\custom\", new[] { "subfolder" });   //duplicate name
            (string parent, string[] subfolders) dir2 = (anotherPath2, new[] { "subfolder" }); //duplicate name
            (string parent, string[] subfolders) dir2a = (@"z:\custooooom2", new[] { "subfolder" }); //duplicate name

            fileAccessor.SetupDirectories(baseDir, dir1, dir2, dir1a, dir2a);

            // Act
            var foldersToMove = FinderProxy.CalculateFoldersToMove_Test(basePath, affectedFolders, fileAccessor);

            // Assert
            Assert.AreEqual(2, foldersToMove.Count());
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir1.parent + dir1.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.Any(m => m.source == (dir2.parent + dir2.subfolders[0]).AddDirSeparator()));
            Assert.IsTrue(foldersToMove.First().destination != foldersToMove.Skip(1).First().destination);
            Assert.IsTrue(foldersToMove.All(f => f.destination != (baseDir.parent + baseDir.subfolders[0]).AddDirSeparator()));
        }
    }
}
