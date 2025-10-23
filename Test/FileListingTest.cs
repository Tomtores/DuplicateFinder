using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Engine.FileEnumerators;
using System.Linq;

namespace Test
{
    [TestClass]
    public class FileAccessTest
    {
        private const string SourceFile = @"..\..\test data\regular1.bmp";
        private const string TestDataPath = @"..\..\test data";
        private const string RestrictedTestDataPath = @"..\..\TestDataRestricted";
        private string[] RestrictedTestDataPaths = new[] { @"..\..\TestDataRestricted" };
        private const string regular = "regular.bmp";
        private const string restricted = "restricted.bmp";   //should not be able to open
        private const string hidden = "hidden.bmp"; //should not list?
        private const string nested_regular = @"\nested\regular.bmp";
        private const string forbidden_regular = @"\forbidden\regular.bmp";   //should not be able to navigate

        private readonly IFileEnumerator enumerator;

        public FileAccessTest()
        {
            this.enumerator = new SafeFileEnumerator();
        }

        [TestInitialize]
        public void PrepareRestrictedFiles()
        {
            NukeRestrictedData();

            CreateRestrictedData();
        }

        /// <summary>
        /// Creates data with restricted access. Sets up the file folder permissions.
        /// </summary>
        private void CreateRestrictedData()
        {
            Directory.CreateDirectory(RestrictedTestDataPath);

            //regular file
            File.Copy(SourceFile, Path.Combine(RestrictedTestDataPath, restricted));

            //hidden file
            var hiddenFile = Path.Combine(RestrictedTestDataPath, hidden);
            File.Copy(SourceFile, hiddenFile);
            File.SetAttributes(hiddenFile, FileAttributes.Hidden);
        }

        /// <summary>
        /// Nuke the existing test data. Overrides any permissions.
        /// </summary>
        private static void NukeRestrictedData()
        {
            if (Directory.Exists(RestrictedTestDataPath))
            {
                Directory.Delete(RestrictedTestDataPath, true);
            }
        }

        /// <summary>
        /// This test checks precondition: One folder that rejects access must be present in test area.
        /// Warning: Folder access rights most likely don't survive across archiving/source control.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void VerifyFoldersAreForbidden()
        {
            var files = Directory.GetFiles(RestrictedTestDataPath, "*.*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// This test checks precondition: one hidden file must exist in the test zone.
        /// Warning: File access most likely doesn't survive across archiving/source control.
        /// </summary>
        [TestMethod]
        public void VerifyHiddenFile()
        {
            var files = Directory.GetFiles(RestrictedTestDataPath, "*.*");

            var hiddenFile = new FileInfo(files.Single(f => f.EndsWith(hidden)));
            if (!hiddenFile.Attributes.HasFlag(FileAttributes.Hidden))
            {
                Assert.Fail("File " + hidden + " should be hidden");
            }

        }

        /// <summary>
        /// This test checks precondition: Exacly one file with rejected access must exist in the test zone.
        /// Warning: File access most likely doesn't survive across archiving/source control.
        /// </summary>
        [TestMethod]
        public void VerifyFileIsForbidden()
        {
            var files = Directory.GetFiles(RestrictedTestDataPath, "*.*");

            var restrictedFile = new FileInfo(files.Single(f => f.EndsWith(restricted)));
            try
            {
                var stream = restrictedFile.OpenRead();   //should throw
            }
            catch (UnauthorizedAccessException)
            {
                //all is well.
                return;
            }

            Assert.Fail("Should have thrown unauthorized access exception");
        }

        [TestMethod]
        public void GivenFileStructureWithRestrictedAccess_ShouldListAllAllowedFolders()
        {
            var files = enumerator.EnumerateFiles(RestrictedTestDataPaths, "*.*");

            Assert.IsTrue(files.Any(f => f.EndsWith(nested_regular)));
        }

        [TestMethod]
        public void GivenFileStructureWithRestrictedAccess_ShouldListAllAllowedFiles()
        {
            var files = enumerator.EnumerateFiles(RestrictedTestDataPaths, "*.*");

            Assert.AreEqual(4, files.Count());
            Assert.IsTrue(files.Any(f => f.EndsWith(nested_regular)));
            Assert.IsTrue(files.Any(f => f.EndsWith(regular)));
            Assert.IsTrue(files.Any(f => f.EndsWith(hidden)));
            Assert.IsTrue(files.Any(f => f.EndsWith(restricted)));
        }
    }
}
