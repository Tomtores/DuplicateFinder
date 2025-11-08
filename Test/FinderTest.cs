using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Entities;
using Engine.FileEnumerators;
using Engine.HashCalculators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class FinderTest
    {
        private const string Trash = @"X:\trashPath";
        private const string Other = @"Y:\otherPath";
        private const string Keep = @"Z:\keepPath";

        private List<string> trashListItems
        {
            get { return new List<string>() { Trash }; }
        }

        private List<string> keepListItems
        {
            get { return new List<string>() { Keep }; }
        }

        private FinderProxy Finder { get; set; }

        public FinderTest()
        {
            this.Finder = new FinderProxy(new StandardFileEnumerator(), new FakeHasher());
        }

        #region deletion List

        [TestMethod]
        public void CalculateDeletions_GivenListWithOneDupePair_WhereOneDupeIsInTrashList_ShouldreturnOneDupe()
        {
            var dupe1 = Item(Trash, "AAA");
            var dupe2 = Item(Other, "BBB");
            var list = MakeList(dupe1, dupe2);

            var result = Finder.Test_CalculateDeletionList(list, trashListItems);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(dupe1.FullName, result.First());
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOnepair_WithOneOutsideAndManyInsideTrashzone_ShouldNukeAllInTrashzone()
        {
            var inside1 = Item(Trash, "AAA");
            var inside2 = Item(Trash, "BBB");
            var outside1 = Item(Other, "CCC");
            var list = MakeList(inside1, inside2, outside1);

            var result = Finder.Test_CalculateDeletionList(list, trashListItems);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(i => i.Equals(inside1.FullName)));
            Assert.IsTrue(result.Any(i => i.Equals(inside2.FullName)));
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOnepair_WithAllInsideTrashzone_ShouldDeleteNothing()
        {
            var inside1 = Item(Trash, "AAA");
            var inside2 = Item(Trash, "BBB");
            var list = MakeList(inside1, inside2);

            var result = Finder.Test_CalculateDeletionList(list, trashListItems);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOnepair_WithAllInsideKeepzone_ShouldKeepAll()
        {
            var inside1 = Item(Keep, "AAA");
            var inside2 = Item(Keep, "BBB");
            var list = MakeList(inside1, inside2);

            var result = Finder.Test_CalculateDeletionList(list, null, keepListItems);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOnepair_WithManyInsideKeepzone_ShouldDeleteOutsideKeepzoneOnly()
        {
            var inside1 = Item(Keep, "AAA");
            var inside2 = Item(Keep, "BBB");
            var outside = Item(Other, "CCC");
            var list = MakeList(inside1, inside2, outside);

            var result = Finder.Test_CalculateDeletionList(list, null, keepListItems);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.First(), outside.FullName);
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOneDupePair_WhereOneDupeIsInKeepList_ShouldreturnOneDupe()
        {
            var dupe1 = Item(Other, "AAA");
            var dupe2 = Item(Keep, "BBB");
            var list = MakeList(dupe1, dupe2);

            var result = Finder.Test_CalculateDeletionList(list, null, keepListItems);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(dupe1.FullName, result.First());
        }

        [TestMethod]
        public void CalculateDeletions_GivenListWithOnepair_WithOneInsideAndManyOutsideKeepzone_ShouldNukeAllOutside()
        {
            var outside2 = Item(Other, "AAA");
            var inside2 = Item(Keep, "BBB");
            var outside1 = Item(Other, "CCC");
            var list = MakeList(outside2, inside2, outside1);

            var result = Finder.Test_CalculateDeletionList(list, null, keepListItems);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(i => i.Equals(outside1.FullName)));
            Assert.IsTrue(result.Any(i => i.Equals(outside2.FullName)));
        }

        [TestMethod]
        public void CalculateDeletions_GivenTwoPairs_OneWithTrashzoneItem_SecondWithKeepItem_ShouldCorrectlyResolveBoth()
        {
            var inside1 = Item(Keep, "AAA");
            var other1 = Item(Other, "BBB");    //del

            var outside2 = Item(Trash, "CCC");  //del
            var other2 = Item(Other, "DDD");

            var inside3 = Item(Keep, "EEE");
            var outside3 = Item(Trash, "FFF");  //del

            var list = MakeList(inside1, other1);
            list.AddRange(MakeList(outside2, other2));
            list.AddRange(MakeList(inside3, outside3));

            var result = Finder.Test_CalculateDeletionList(list, trashListItems, keepListItems);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains(other1.FullName));
            Assert.IsTrue(result.Contains(outside2.FullName));
            Assert.IsTrue(result.Contains(outside3.FullName));
        }

        /// <summary>
        /// BUG: When we mark "c:\keep" as keepzone, but have dupe in "C:\keep1", program thinks both folders are keepzone.
        /// </summary>
        [TestMethod]
        public void CalculateDeletions_KeepzoneShouldmatchOnExactFolderName()
        {
            var inside = Item(Keep, "Inside");
            var outside = Item(Keep + "123", "OtherFolder");

            var list = MakeList(inside, outside);

            var result = Finder.Test_CalculateDeletionList(list, null, keepListItems);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(outside.FullName, result.First());        
        }

        #endregion

        [TestMethod]
        public void AfterDelete_GivenSinglePair_WithOneItemleft_ShouldReturnEmpty()
        {
            var item1 = Item(Trash, "AAA");
            var item2 = Item(Other, "BBB");
            var list = MakeList(item1, item2);
            var delitems = new List<string> { item1.FullName };

            var result = this.Finder.Test_TrimDeleted(list, delitems);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void AfterDelete_GivenSinglePair_WithTwoItemsleft_ShouldReturnRemainingItems()
        {
            var item1 = Item(Trash, "AAA");
            var item2 = Item(Other, "BBB");
            var item3 = Item(Other, "CCC");
            var list = MakeList(item1, item2, item3);
            var delitems = new List<string> { item1.FullName };

            var result = this.Finder.Test_TrimDeleted(list, delitems);

            var first = result.First();
            Assert.AreEqual(2, first.Count());
            Assert.IsTrue(first.Any(i => i.FullName == item2.FullName));
            Assert.IsTrue(first.Any(i => i.FullName == item3.FullName));
        }

        private Duplicate Item(string path, string filename)
        {
            return new Duplicate(path + @"\" + filename, 30, DateTime.Now);
        }

        private static List<Duplicate[]> MakeList(params Duplicate[] dupes)
        {
            return new List<Duplicate[]>() { dupes };
        }
    }

    internal class FinderProxy : Finder
    {
        public FinderProxy(IFileEnumerator finder, params IHashCalculator[] hashers)
            : base(finder, hashers)
        {
        }

        public IEnumerable<string> Test_CalculateDeletionList(IEnumerable<Duplicate[]> items, IEnumerable<string> trashList = null, IEnumerable<string> keepList = null)
        {
            base.duplicates.Replace(items.ToList());
            return base.CalculateFilesToDelete(trashList, keepList);
        }

        public IEnumerable<Duplicate[]> Test_TrimDeleted(List<Duplicate[]> list, List<string> delitems)
        {
            base.duplicates.Replace(list);
            base.DeleteItems(delitems);
            return base.Duplicates;
        }
    }
}
