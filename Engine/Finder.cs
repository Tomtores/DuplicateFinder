using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Engine.Entities;
using Engine.FileEnumerators;
using Engine.HashCalculators;
using Microsoft.VisualBasic.FileIO;

namespace Engine
{
    internal class Finder : IFinder
    {
        private readonly IHashCalculator[] hashers;
        private readonly IFileEnumerator fileFinder;
        private ProgressStatus status;
        private Action<ProgressKind> progressUpdateCallback;

        private void NotifyProgress(ProgressKind kind) => this.progressUpdateCallback(kind);

        public IProgressStatus Status
        {
            get
            {
                // todo generate status
                return this.status;
            }
        }

        protected ConcurrentBufferedList<Duplicate[]> duplicates = new ConcurrentBufferedList<Duplicate[]>();

        public IEnumerable<Duplicate[]> Duplicates
        {
            get
            {
                return this.duplicates.Results;
            }
        }

        public Finder(IFileEnumerator finder, params IHashCalculator[] hashers)
        {
            this.fileFinder = finder;
            this.hashers = hashers ?? new IHashCalculator[] { };
            this.status = new ProgressStatus();
        }

        private void Report(WorkState state)
        {
            this.status.State = state;
            NotifyProgress(ProgressKind.StatusMessage);
        }

        private void Report(string message = null, int? filesFound = null, long totalSize = 0, long processedSize = 0)
        {
            var prevValues = new { this.status.Message, this.status.Progress, this.status.FilesFound };

            int? progress = null;
            if (totalSize != 0)
            {
                var processed = (double)processedSize / totalSize;
                progress = (int)Math.Ceiling(processed * 100); //Ceil prevents fraction drops from leaving bar at that annoying 99% complete.
            }

            this.status.Message = message ?? this.status.Message;
            this.status.Progress = progress ?? this.Status.Progress;
            this.status.FilesFound = filesFound ?? this.Status.FilesFound;

            if (prevValues.Message != this.status.Message || prevValues.Progress != this.status.Progress || prevValues.FilesFound != this.status.FilesFound)
            {
                NotifyProgress(ProgressKind.StatusMessage);
            }
        }

        private void ClearState()
        {
            this.status = new ProgressStatus();
            this.duplicates.Clear();
        }

        /// <summary>
        /// Scans selected path for duplicate files.
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="filter"></param>
        /// <param name="ignored"></param>
        public void FindDuplicates(string[] paths, string filter, string[] ignored, Action<ProgressKind> progressUpdateCallback = null, bool skipEmpty = false, int? minSizeKB = null, int? maxSizeKB = null)
        {
            this.progressUpdateCallback = progressUpdateCallback ?? (s => { });

            try
            {
                this.ClearState();
                this.Report(WorkState.ListingFiles);
                var files = this.ListFiles(paths, filter, ignored);

                var possibleDuplicates = this.FindDuplicateSizes(files, skipEmpty, minSizeKB, maxSizeKB);

                // Ensure the fileinfo list gets dropped - we no longer need the data, and it's the biggest object in memory now.
                files.Clear();
                GC.Collect();

                this.Report(WorkState.Comparing);
                this.FindDuplicateContents(possibleDuplicates);

                this.Report(WorkState.Done);
                return;
            }
            catch (Exception)
            {
                this.Report(WorkState.Error);
                throw;
            }
        }

        private IList<Duplicate> ListFiles(string[] paths, string filter, string[] ignores)
        {
            var result = new List<Duplicate>();
            var filesIn = this.fileFinder.EnumerateFiles(paths, filter);

            foreach (var file in filesIn)
            {
                if (!ignores.Any(i => file.Contains(i)))
                {
                    try
                    {
                        var info = new FileInfo(file);
                        var duplicate = new Duplicate(info.FullName, info.Length, info.LastWriteTimeUtc);
                        result.Add(duplicate);
                        this.Report(duplicate.FullName, result.Count);
                    }
                    catch (PathTooLongException e)
                    {
                        throw new PathTooLongException(file, e);
                    }
                    catch (FileNotFoundException e)
                    {
                        throw new FileNotFoundException(file, e);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Scan files and return collection of ones with duplicate sizes.
        /// </summary>
        private IEnumerable<Duplicate[]> FindDuplicateSizes(IEnumerable<Duplicate> fileinfo, bool skipEmpty, int? minSizeKB, int? maxSizeKB)
        {
            var filtered = fileinfo;

            // No point in optimizing following, as size is already read and stored in memory.
            if (skipEmpty)
            {
                filtered = filtered.Where(f => f.Size > 0);
            }
            if (minSizeKB.HasValue)
            {
                filtered = filtered.Where(f => f.Size >= minSizeKB.Value * 1024);
            }
            if (maxSizeKB.HasValue)
            {
                filtered = filtered.Where(f => f.Size <= maxSizeKB.Value * 1024);
            }

            var groups = filtered.GroupBy(f => f.Size);
            var duplicateSets = groups.Where(g => g.Count() > 1);
            var duplicateList = duplicateSets.Select(d => d.ToArray()).ToList();

            this.Report(filesFound: duplicateList.Sum(d => d.Length));

            return duplicateList;
        }

        /// <summary>
        /// Scan files and return ones with same content.
        /// </summary>
        private void FindDuplicateContents(IEnumerable<Duplicate[]> duplicates)
        {
            var count = duplicates.Sum(d => d.Length);
            var totalSize = duplicates.Sum(d => d.Sum(i => i.Size));
            this.Report(filesFound: count);

            var processedSize = 0L;

            //ToDo: change processing from foreach to cascade. Use first hasher on the collection and evict entries with no macth. Then run the second hasher for all items.
            //change reporting to account for multiple passes.

            foreach (var duplicate in duplicates)
            {
                this.Report(duplicate.First().FullName);
                var result = this.CascadeCompare(duplicate, this.hashers);
                processedSize += duplicate.Sum(d => d.Size);
                
                if (result != null && result.Any())
                {
                    this.duplicates.AddItems(result);
                    NotifyProgress(ProgressKind.ItemList);
                }

                this.Report(null, null, totalSize, processedSize);
            }
        }

        private IEnumerable<Duplicate[]> CascadeCompare(Duplicate[] duplicates, IEnumerable<IHashCalculator> hashers)
        {
            IEnumerable<Duplicate[]> currentResult = new List<Duplicate[]>() { duplicates };
            foreach (var hasher in hashers)
            {
                var innerResult = new List<Duplicate[]>();
                foreach (var group in currentResult)
                {
                    var result = this.GroupCompare(group, hasher);
                    innerResult.AddRange(result);
                }
                currentResult = innerResult;
            }

            return currentResult;
        }

        private IEnumerable<Duplicate[]> GroupCompare(Duplicate[] duplicates, IHashCalculator hasher)
        {
            foreach (var duplicate in duplicates)
            {
                duplicate.Hash = hasher.ComputeHash(duplicate);
            }

            var grouped = duplicates.GroupBy(g => g.Hash);

            return grouped.Where(g => g.Count() > 1).Select(g => g.ToArray());
        }

        /// <summary>
        /// Calculate items that can be resolved with given trash and keep folders.
        /// </summary>
        public IEnumerable<string> CalculateFilesToDelete(IEnumerable<string> trashList = null, IEnumerable<string> keepList = null)
        {
            this.Report(WorkState.Marking);
            var deletions = new List<string>();
            try
            {
                IEnumerable<Duplicate[]> items = this.duplicates.Results;
                if (keepList != null && keepList.Any())
                {
                    var keepDeletions = CalculateDeletionKeepList(items, keepList);
                    deletions.AddRange(keepDeletions);
                    items = CalculateAfterDeleteResults(items, deletions);
                }

                if (trashList != null && trashList.Any())
                {
                    var trashDeletions = CalculateDeletionTrashList(items, trashList);
                    deletions.AddRange(trashDeletions);
                }
            }
            catch (Exception)
            {
                this.Report(WorkState.Error);
                throw;
            }

            this.Report(WorkState.Iddle);
            return deletions;
        }

        private IEnumerable<string> CalculateDeletionTrashList(IEnumerable<Duplicate[]> items, IEnumerable<string> trashList)
        {
            var results = new List<string>();
            var count = items.Count();
            var current = 1;

            foreach (var item in items)
            {
                this.Report("Marking: Phase B, trash entry " + current + " of " + count);
                var outside = item.Where(i => !IsInFolder(trashList, i));

                if (outside.Any())  //we have at least one item outside trashzone, thus we can nuke all in trashzone
                {
                    var inside = item.Where(i => IsInFolder(trashList, i));
                    results.AddRange(inside.Select(i => i.FullName));
                }
                current++;
            }

            return results;
        }

        private IEnumerable<string> CalculateDeletionKeepList(IEnumerable<Duplicate[]> items, IEnumerable<string> keepList)
        {
            var results = new List<string>();
            var count = items.Count();
            var current = 1;

            foreach (var item in items)
            {
                this.Report("Marking: Phase A, keep entry " + current + " of " + count);
                var inside = item.Where(i => IsInFolder(keepList, i));

                if (inside.Any()) //we have at least one item in keep zone, thus we can nuke everything outside
                {
                    var outside = item.Where(i => !IsInFolder(keepList, i));
                    results.AddRange(outside.Select(i => i.FullName));
                }
                current++;
            }

            return results;
        }

        private static bool IsInFolder(IEnumerable<string> folderList, Duplicate i)
        {
            return folderList.NullSafe().Any(f => i.DirectoryName.AddDirSeparator().StartsWith(f.AddDirSeparator()));
        }

        /// <summary>
        /// Cleans the given list from files that were deleted and returns a new list.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="deleted"></param>
        /// <returns></returns>
        private static IEnumerable<Duplicate[]> CalculateAfterDeleteResults(IEnumerable<Duplicate[]> results, IEnumerable<string> deleted)
        {
            var withoutdeleted = results.Select(i => i.Where(r => deleted.All(d => d != r.FullName)).ToArray());
            var nonSingularGroups = withoutdeleted.Where(i => i.Length > 1);
            return nonSingularGroups;
        }

        /// <summary>
        /// Deletes given files from duplicate list and trims orphaned entries.
        /// </summary>
        /// <param name="deleted"></param>
        [Obsolete("Delete this code in next version")]
        public IEnumerable<Exception> DeleteItems(IEnumerable<string> toDelete)
        {
            var errors = new List<Exception>();
            this.Report(WorkState.Deleting);
            try
            {
                var totalFiles = toDelete.Count();
                var currentFile = 1;

                foreach (var item in toDelete)
                {
                    this.Report("Deleting file " + currentFile + " of " + totalFiles, null, totalFiles, currentFile);
                    try
                    {
                        if (File.Exists(item))
                        {
                            if (File.GetAttributes(item).HasFlag(FileAttributes.ReadOnly))
                            {
                                errors.Add(new UnauthorizedAccessException(item));
                            }
                            else
                            {
                                FileSystem.DeleteFile(item, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            }
                        }
                        else
                        {
                            errors.Add(new FileNotFoundException(item));
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }

                    currentFile++;
                }

                var results = CalculateAfterDeleteResults(this.duplicates.Results, toDelete);
                this.duplicates.Replace(results.ToList());
                NotifyProgress(ProgressKind.ItemList);

                this.Report(WorkState.Iddle);
            }
            catch (Exception)
            {
                this.Report(WorkState.Error);
                throw;
            }

            return errors;
        }       

        public async Task<IEnumerable<Exception>> DeleteItemsAsync(IEnumerable<string> toDelete, IProgress<(int total, int processed, string currentFile)> progressCallback, CancellationToken cancellationToken)
        {
            var errors = new List<Exception>();
            this.Report(WorkState.Deleting);    // todo get rid of reporting later
            var totalFiles = toDelete.Count();

            try
            {                
                var currentFile = 1;

                foreach (var item in toDelete)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        this.Report(WorkState.Error);
                        return new[] { new OperationCanceledException() };
                    }

                    this.Report("Deleting file " + currentFile + " of " + totalFiles, null, totalFiles, currentFile);   // todo get rid of reporting later
                    progressCallback.Report((totalFiles, currentFile - 1, item));

                    try
                    {
                        // not checking if file exists in duplicate list for performance reasons - I trust caller to not try and delete wrong things here.
                        if (File.Exists(item))
                        {
                            if (File.GetAttributes(item).HasFlag(FileAttributes.ReadOnly))
                            {
                                errors.Add(new UnauthorizedAccessException(item));
                            }
                            else
                            {
                                FileSystem.DeleteFile(item, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            }
                        }
                        else
                        {
                            errors.Add(new FileNotFoundException(item));
                        }
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }

                    currentFile++;
                }

                var results = CalculateAfterDeleteResults(this.duplicates.Results, toDelete);
                this.duplicates.Replace(results.ToList());
                NotifyProgress(ProgressKind.ItemList);

                this.Report(WorkState.Iddle);                
            }
            catch (Exception)
            {
                this.Report(WorkState.Error);
                throw;
            }

            progressCallback.Report((totalFiles, totalFiles, null));

            return errors;
        }
    }
}
