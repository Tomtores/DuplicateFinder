using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Entities;
using Engine.FileEnumerators;
using Engine.HashCalculators;

namespace Test
{
    internal class FinderProxy : Finder
    {
        public FinderProxy(IFileAccessor finder, params IHashCalculator[] hashers)
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
            base.DeleteItemsAsync(delitems, new Progress<(int total, int processed, string currentFile)>(), System.Threading.CancellationToken.None).Wait();
            return base.Duplicates;
        }

        public static IEnumerable<string> CalculateAffectedFolders_Test(string folderPath, IEnumerable<string> toDelete) 
            => CalculateAffectedFolders(folderPath, toDelete);

        public static IEnumerable<(string source, string destination)> CalculateFilesToMove_Test(string targetFolderPath, IEnumerable<string> affectedFolders, IEnumerable<string> duplicatesToDelete, IFileAccessor fileAccessor) 
            => CalculateFilesToMove(targetFolderPath, affectedFolders, duplicatesToDelete, fileAccessor);

        public static IEnumerable<(string source, string destination)> CalculateFoldersToMove_Test(string targetFolderPath, IEnumerable<string> affectedFolders, IFileAccessor fileAccessor)
            => CalculateFoldersToMove(targetFolderPath, affectedFolders, fileAccessor);
    }
}
