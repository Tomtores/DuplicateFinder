using System.Collections.Generic;
using Engine.Entities;
using System;

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
        /// Deletes given files from duplicate list and trims orphaned entries.
        /// </summary>
        /// <param name="deleted"></param>
        IEnumerable<Exception> DeleteItems(IEnumerable<string> deleted);

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