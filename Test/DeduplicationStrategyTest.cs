using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.CleanupStrategies;
using Engine.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Test
{
    [TestClass]
    public class DeduplicationStrategyTest
    {
        [TestMethod]
        public void RemoveDuplicatesWithinFolder_GivenFolderWithDuplicates_ShouldMarkAllButFirstorDeletion()
        {
            var strategy = new RemoveExtraCopiesWithinFolderStrategy();

            var dupes = new List<Duplicate[]>()
            {
               new [] { Make("folder1", "file3.aaa"), Make("folder1", "file2.aaa"), Make("folder1", "file1.aaa") },
               new [] {Make("folder2", "file1.aaa"), Make("folder2", "file2.aaa"), Make("folder2\\subfolder", "bbb.aaa")},
            };

            var result = strategy.MarkTrash(dupes);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains(dupes[0][0].FullName));
            Assert.IsTrue(result.Contains(dupes[0][1].FullName));
            Assert.IsTrue(result.Contains(dupes[1][1].FullName));
        }

        private static Duplicate Make(string folder, string filename)
        {
           return new Duplicate(folder + "\\" + filename, 10, DateTime.Now);
        }
    }
}
