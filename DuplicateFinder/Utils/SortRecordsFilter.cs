using DuplicateFinder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuplicateFinder.Utils
{
    internal class SortRecordsFilter
    {
        private static Lazy<FolderFileCountCache> _countCache = new Lazy<FolderFileCountCache>(() => FolderFileCountCache.GetInstance());

        public static IEnumerable<DuplicateViewItem[]> Execute(IEnumerable<DuplicateViewItem[]> results, ColumnNames column, bool ascending)
        {
            if (column == ColumnNames.Size)
            {
                Func<DuplicateViewItem[], long> sizeSort = i => i.First().Size;
                if (ascending)
                {
                    results = results.OrderBy(sizeSort);
                }
                else
                {
                    results = results.OrderByDescending(sizeSort);
                }
            }

            // Warning - this is very heavy to run
            if (column == ColumnNames.FolderCount)
            {
                var withPrecomputerFolderCounts = results.Select(r => r.Select(ai => new { item = ai, folderCount = _countCache.Value.GetDirectoryFileCount(ai.DirectoryName) }));

                if (ascending)
                {
                    results = withPrecomputerFolderCounts.Select(arr => arr.OrderBy(item => item.folderCount).ToArray())
                        .OrderBy(arr => arr.First().folderCount).ThenBy(arr => arr.Last().folderCount)
                        .Select(arr => arr.Select(ai => ai.item).ToArray());
                }
                else
                {
                    results = withPrecomputerFolderCounts.Select(arr => arr.OrderByDescending(item => item.folderCount).ToArray())
                        .OrderByDescending(arr => arr.First().folderCount).ThenByDescending(arr => arr.Last().folderCount)
                        .Select(arr => arr.Select(ai => ai.item).ToArray());
                }
            }

            if (column == ColumnNames.Count)
            {
                if (ascending)
                {
                    results = results.Select(i => i.OrderBy(e => e.DirectoryDuplicatesCount).ToArray()).OrderBy(i => i.First().DirectoryDuplicatesCount).ThenBy(i => i.Last().DirectoryDuplicatesCount);
                }
                else
                {
                    results = results.Select(i => i.OrderByDescending(e => e.DirectoryDuplicatesCount).ToArray()).OrderByDescending(i => i.First().DirectoryDuplicatesCount).ThenByDescending(i => i.Last().DirectoryDuplicatesCount);
                }
            }

            if (column == ColumnNames.Path)
            {
                if (ascending)
                {
                    results = results.Select(i => i.OrderBy(e => e.FullName).ToArray()).OrderBy(i => i.First().FullName);
                }
                else
                {
                    results = results.Select(i => i.OrderByDescending(e => e.FullName).ToArray()).OrderByDescending(i => i.First().FullName);
                }
            }

            return results;
        }

    }
}
