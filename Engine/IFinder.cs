using System.Collections.Generic;
using Engine.Entities;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Engine
{
    public interface IFinder
    {
        /// <summary>
        /// Scans selected path for duplicate files.
        /// </summary>
        /// <param name="paths">Root folder to begin search from.</param>
        /// <param name="filter">Filename/extension filter.</param>
        /// <param name="ignored">List of ignored file/folder names.</param>
        void FindDuplicates(string[] paths, string filter, string[] ignored, Action<ProgressKind> progressUpdateCallback = null, bool skipEmpty = false, int? minSizeKB = null, int? maxSizeKB = null);

        /// <summary>
        /// Calculate items that can be resolved with given trash and keep folders.
        /// </summary>
        IEnumerable<string> CalculateFilesToDelete(IEnumerable<string> trashList = null, IEnumerable<string> keepList = null);

        /// <summary>
        /// Deletes given files from disk and duplicate list and trims orphaned entries asynchronously.
        /// </summary>
        /// <param name="toDelete">List of file paths to delete.</param>
        /// <param name="progressCallback">Callback for reporting progress.</param>
        /// <returns>List of exceptions that occured during deletion.</returns>
        Task<IEnumerable<Exception>> DeleteItemsAsync(IEnumerable<string> toDelete, IProgress<(int total, int processed, string currentFile)> progressCallback, CancellationToken cancellationToken);

        /// <summary>
        /// Reports current finder state.
        /// </summary>
        IProgressStatus Status { get; }

        /// <summary>
        /// List of duplicates found.
        /// </summary>
        IEnumerable<Duplicate[]> Duplicates { get; }
    }
}