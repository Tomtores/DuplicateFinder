using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test.MergeFoldersTests
{
    [TestClass]
    public class CalculateAffectedFolders_Tests
    {
        private string basePath = @"x:\test\";
        private string anotherPath = @"x:\otherDir\";

        [TestMethod]
        public void GivenEmptyDuplicateList_ShouldReturnEmpty()
        {
            var dupeFiles = Enumerable.Empty<string>();
            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(0, affectedFolders.Count());
        }

        [TestMethod]
        public void GivenDuplicateInTargetFolder_ShouldReturnEmpty()
        {
            var dupeFiles = new[]
            {
                basePath + @"file1.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(0, affectedFolders.Count());
        }

        [TestMethod]
        public void GivenDuplicateInSubfolderOfTarget_ShouldReturnEmpty()   // this cannot happen I think?
        {
            var dupeFiles = new[]
            {
                basePath + @"subfolder\file1.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(0, affectedFolders.Count());
        }


        [TestMethod]
        public void GivenDuplicateInAnotherFolder_ShouldListOtherFolderForMove()
        {
            var dupeFiles = new[]
            {
                basePath + @"file1.txt",
                anotherPath + @"file2.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(1, affectedFolders.Count());
            Assert.AreEqual(anotherPath, affectedFolders.Single());
        }

        [TestMethod]
        public void GivenDuplicateInAnotherFolderAndItsSubfolder_ShouldListParentFolderForMove()
        {
            var dupeFiles = new[]
            {
                basePath + @"file1.txt",
                anotherPath + @"file2.txt",
                anotherPath + @"subfolder\file2.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(1, affectedFolders.Count());
            Assert.AreEqual(anotherPath, affectedFolders.Single());
        }

        [TestMethod]
        public void GivenDuplicateInSeparateSubfolders_ShouldListSeparateFoldersForMove()
        {
            var dupeFiles = new[]
            {
                basePath + @"file1.txt",
                anotherPath + @"subfolder1\file2.txt",
                anotherPath + @"subfolder2\file2.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(2, affectedFolders.Count());
            Assert.IsTrue(affectedFolders.Contains(anotherPath + @"subfolder1\"));
            Assert.IsTrue(affectedFolders.Contains(anotherPath + @"subfolder2\"));
        }

        [TestMethod]
        public void GivenMultipleHierarchicalFolders_ShouldListParentFoldersForMove()
        {
            var dupeFiles = new[]
            {
                anotherPath + @"subfolderA\subsubfolder\text.text",
                basePath + @"file1.txt",
                anotherPath + @"file2.txt",
                anotherPath + @"subfolder\file2.txt",
                basePath + @"subfolderB\file3.txt",
            };

            var affectedFolders = FinderProxy.CalculateAffectedFolders_Test(basePath, dupeFiles);

            Assert.AreEqual(1, affectedFolders.Count());
            Assert.AreEqual(anotherPath, affectedFolders.Single());
        }
    }
}
