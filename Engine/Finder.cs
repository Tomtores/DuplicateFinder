using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Engine.Entities;
using Engine.FileEnumerators;
using Engine.HashCalculators;

namespace Engine
{
    internal class Finder : IFinder
    {
        private readonly IHashCalculator[] hashers;
        private readonly IFileAccessor fileAccessor;
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

        public Finder(IFileAccessor finder, params IHashCalculator[] hashers)
        {
            this.fileAccessor = finder;
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
            var filesIn = this.fileAccessor.EnumerateFiles(paths, filter, recursive: true);

            foreach (var file in filesIn)
            {
                if (!ignores.Any(i => file.Contains(i)))
                {
                    try
                    {
                        var (FullName, Length, LastWriteTimeUtc) = fileAccessor.GetFileInfo(file);
                        var duplicate = new Duplicate(FullName, Length, LastWriteTimeUtc);
                        result.Add(duplicate);
                        this.Report(duplicate.FullName, result.Count);
                    }
                    catch (System.IO.PathTooLongException e)
                    {
                        throw new System.IO.PathTooLongException(file, e);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        throw new System.IO.FileNotFoundException(file, e);
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
                    var keepDeletions = CalculateDeletionKeepList(items, keepList, msg => this.Report(msg));
                    deletions.AddRange(keepDeletions);
                    items = CalculateAfterDeleteResults(items, deletions);
                }

                if (trashList != null && trashList.Any())
                {
                    var trashDeletions = CalculateDeletionTrashList(items, trashList, msg => this.Report(msg));
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

        /// <summary>
        /// Returns list of duplicates that can be deleted because they exist in trash directories, but have copy in directories we are keeping.
        /// </summary>        
        private static IEnumerable<string> CalculateDeletionTrashList(IEnumerable<Duplicate[]> items, IEnumerable<string> trashList, Action<string> reportProgressMessage)
        {
            var results = new List<string>();
            var count = items.Count();
            var current = 1;

            foreach (var item in items)
            {
                reportProgressMessage("Marking: Phase B, trash entry " + current + " of " + count);
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

        /// <summary>
        /// Returns a list of duplicates that can be deleted because they exist outside 'keep' directory.
        /// </summary>
        private static IEnumerable<string> CalculateDeletionKeepList(IEnumerable<Duplicate[]> items, IEnumerable<string> keepList, Action<string> reportProgressMessage)
        {
            var results = new List<string>();
            var count = items.Count();
            var current = 1;

            foreach (var item in items)
            {
                reportProgressMessage("Marking: Phase A, keep entry " + current + " of " + count);
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
                        fileAccessor.DeleteFile(item);
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

        public MergePreview CalculateMergeIntoFolder(string targetFolderPath)
        {
            if (this.Duplicates == null)
            {
                throw new InvalidOperationException("No duplicates have been loaded yet");
            }

            // enumerate all files in other folders that are duplicates to remove
            var duplicatesToDelete = CalculateDeletionKeepList(this.Duplicates, new[] { targetFolderPath }, msg => { });    // todo add option to skip searching duplicates in subfolders and only handle the top level directory

            var affectedOtherFolders = CalculateAffectedFolders(targetFolderPath, duplicatesToDelete);

            // enumerate all the files in other folders that are not duplicates, to move them into merge folder
            var filesToMove = CalculateFilesToMove(targetFolderPath, affectedOtherFolders, duplicatesToDelete, this.fileAccessor);

            // enumerate all standalone folders to move into merge folder
            var foldersToMove = CalculateFoldersToMove(targetFolderPath, affectedOtherFolders, this.fileAccessor);

            var hasInvalidFileMoves = filesToMove.Any(f => !Extensions.AreOnSameDrive(f.source, f.destination));
            var hasInvalidDirectoryMoves = foldersToMove.Any(f => !Extensions.AreOnSameDrive(f.source, f.destination));

            return new MergePreview(filesToMove, foldersToMove, duplicatesToDelete, hasInvalidFileMoves, hasInvalidDirectoryMoves);
        }

        /// <summary>
        /// Return list of all folders that had duplicates in them and need the contents moved. If folders are nested, only top level folder will be returned.
        /// </summary>
        protected static IEnumerable<string> CalculateAffectedFolders(string targetFolderPath, IEnumerable<string> toDelete)
        {
            // get list of all folders where duplicates existed, except one we are merging into
            var folderList = toDelete
                .Select(d => Path.GetDirectoryName(d).AddDirSeparator())
                .Where(d => d.StartsWith(targetFolderPath.AddDirSeparator()) == false)
                .Distinct();

            // Ensure no folders are nested within existing folders on the list - the contents will be moved with parent.
            var orderedByNesting = folderList
                .OrderBy(f => f.Count(c => c == Path.DirectorySeparatorChar))
                .ThenBy(f => f);

            var topLevelFolders = new List<string>();
            foreach (var folder in orderedByNesting)
            {
                if (topLevelFolders.Any(tf => folder.StartsWith(tf)))
                {
                    continue;   // a parent folder already exists
                }

                topLevelFolders.Add(folder);
            }

            return topLevelFolders;
        }

        /// <summary>
        /// Calculates file moves. Also handles name conflict renames.
        /// </summary>
        protected static IEnumerable<(string source, string destination)> CalculateFilesToMove(string targetFolderPath, IEnumerable<string> affectedFolders, IEnumerable<string> duplicatesToDelete, IFileAccessor fileAccessor)
        {
            // build a list of what exists in target folder (files)
            var targetFolderFiles = fileAccessor.EnumerateFiles(new[] { targetFolderPath }, "*", recursive: false).ToList();
            var moves = new List<(string source, string destination)>();

            // enumerate files in each source folder, only top level
            foreach (var folder in affectedFolders)
            {
                var folderFiles = fileAccessor.EnumerateFiles(new[] { folder }, "*", recursive: false);

                // filter out duplicates we will be deleting
                var withoutDuplicates = folderFiles.Except(duplicatesToDelete);

                foreach (var file in withoutDuplicates)
                {
                    var newPath = GetFilenameWithoutConflicts(file, targetFolderPath, targetFolderFiles);
                    targetFolderFiles.Add(newPath);
                    moves.Add((file, newPath));
                }                
            }

            return moves;
        }

        protected static IEnumerable<(string source, string destination)> CalculateFoldersToMove(string targetFolderPath, IEnumerable<string> affectedFolders,  IFileAccessor fileAccessor)
        {
            // build a list of what exists in target folder (subfolders)
            var targetFolderSubfolders = fileAccessor.EnumerateDirectories(targetFolderPath)
                .Select(d => d.AddDirSeparator())   // prevent funny business with missing terminators
                .ToList();
            var moves = new List<(string source, string destination)>();

            // enumerate subfolders in each source folder, only top level
            foreach (var folder in affectedFolders)
            {
                var subfolders = fileAccessor.EnumerateDirectories(folder).Select(d => d.AddDirSeparator());

                foreach (var subfolder in subfolders)
                {
                    var newPath = GetDirectoryNameWithoutConflicts(subfolder, targetFolderPath, targetFolderSubfolders);
                    targetFolderSubfolders.Add(newPath);
                    moves.Add((subfolder, newPath));
                }
            }

            return moves;
        }

        private static string GetFilenameWithoutConflicts(string file, string targetFolderPath, List<string> targetFolderFiles)
        {
            var newPath = Path.Combine(targetFolderPath, Path.GetFileName(file));
            int counter = 1;

            while (targetFolderFiles.Contains(newPath))
            {
                newPath = Path.Combine(targetFolderPath, $"{Path.GetFileNameWithoutExtension(file)} ({counter}){Path.GetExtension(file)}");

                counter++;
                if (counter > 1000)
                {
                    throw new InvalidOperationException("Could not compute alternate file name - too many files with same name exist!");
                }
            }

            return newPath;
        }

        private static string GetDirectoryNameWithoutConflicts(string subfolder, string targetFolderPath, List<string> targetFolderSubfolders)
        {
            var subfolderName = Path.GetDirectoryName(subfolder.AddDirSeparator())
                .Split(new char[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .Last();
            var newPath = Path.Combine(targetFolderPath, subfolderName).AddDirSeparator();
            int counter = 1;

            while (targetFolderSubfolders.Contains(newPath))
            {
                newPath = Path.Combine(targetFolderPath, $"{subfolderName} ({counter}){Path.DirectorySeparatorChar}");

                counter++;
                if (counter > 1000)
                {
                    throw new InvalidOperationException("Could not compute alternate folder name - too many folders with same name exist!");
                }
            }

            return newPath;
        }

        public async Task<IEnumerable<Exception>> MergeIntoFolderAsync(MergePreview actionsToPerform, bool moveSubfolders, IProgress<(int total, int processed, string currentFile)> progressCallback, CancellationToken cancellationToken)
        {
            this.Report(WorkState.Merging);

            var total = actionsToPerform.FilesToMove.Count() + actionsToPerform.DuplicatesToDelete.Count() + (moveSubfolders ? actionsToPerform.FoldersToMove.Count() : 0);
            var processed = 0;
            var exceptions = new List<Exception>();

            foreach (var file in actionsToPerform.FilesToMove)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                this.Report("Moving file " + processed + " of " + total, null, total, processed);   // todo get rid of reporting later
                progressCallback.Report((total, processed, file.source));

                try
                {
                    this.fileAccessor.MoveFile(file.source, file.destination);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                processed++;
            }

            // need to drop duplicates before folder move, because some of themn may be in moved folders
            var reportTotalprogress = new Progress<(int total, int processed, string filename)>(p => progressCallback.Report((total, processed + p.processed, p.filename)));

            var errors = await this.DeleteItemsAsync(actionsToPerform.DuplicatesToDelete, reportTotalprogress, cancellationToken);
            exceptions.AddRange(errors);

            processed += actionsToPerform.DuplicatesToDelete.Count();   //fixup local counter

            if (moveSubfolders)
            {
                foreach (var folder in actionsToPerform.FoldersToMove)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    this.Report("Moving folder " + processed + " of " + total, null, total, processed);   // todo get rid of reporting later
                    progressCallback.Report((total, processed, folder.source));

                    try 
                    { 
                        this.fileAccessor.MoveDirectory(folder.source, folder.destination);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }

                    processed++;
                }
            } 
            
            progressCallback.Report((total, processed, null));

            Report(WorkState.Iddle);

            return exceptions;
        }
    }
}
