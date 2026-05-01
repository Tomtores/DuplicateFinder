using Engine.Entities;
using Plugins.Cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Cache
{
    internal class CachedFinderDecorator : IFinder
    {
        private readonly IFinder _finder;
        private readonly IHashCache _cache;

        public CachedFinderDecorator(IFinder finder, IHashCache cache)
        {
            _finder = finder;
            _cache = cache;
        }

        #region Finder interface

        public IProgressStatus Status => _finder.Status;

        public IEnumerable<Duplicate[]> Duplicates => _finder.Duplicates;

        public IEnumerable<string> CalculateFilesToDelete(IEnumerable<string> trashList = null, IEnumerable<string> keepList = null)
        {
            return _finder.CalculateFilesToDelete(trashList, keepList);
        }

        public MergePreview CalculateMergeIntoFolder(string folderPath)
        {
            return _finder.CalculateMergeIntoFolder(folderPath);
        }

        public Task<IEnumerable<Exception>> DeleteItemsAsync(IEnumerable<string> toDelete, IProgress<(int total, int processed, string currentFile)> progressCallback, CancellationToken cancellationToken)
        {
            var result = _finder.DeleteItemsAsync(toDelete, progressCallback, cancellationToken);
            RemoveFromCache(toDelete);
            return result;
        }

        public void FindDuplicates(string[] paths, string filter, string[] ignored, Action<ProgressKind> progressUpdateCallback = null, bool skipEmpty = false, int? minSizeKB = null, int? maxSizeKB = null)
        {
            _finder.FindDuplicates(paths, filter, ignored, progressUpdateCallback, skipEmpty, minSizeKB, maxSizeKB);
            _cache.Flush(); // ensure all results are saved to disk
        }

        public Task<IEnumerable<Exception>> MergeIntoFolderAsync(MergePreview actionsToPerform, bool moveSubfolders, IProgress<(int total, int processed, string currentFile)> progressCallback, CancellationToken cancellationToken)
        {
            var result = _finder.MergeIntoFolderAsync(actionsToPerform, moveSubfolders, progressCallback, cancellationToken);
            RemoveFromCache(actionsToPerform.DuplicatesToDelete);
            return result;
        }

        #endregion

        private void RemoveFromCache(IEnumerable<string> items)
        {
            if (_cache != null)
            {
                try
                {
                    foreach (string item in items)
                    {
                        _cache.Remove(item);
                    }

                    _cache.Flush();
                }
                catch
                {
                    // ignore errors
                }
            }
        }
    }
}
