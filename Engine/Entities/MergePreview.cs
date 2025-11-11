using System.Collections.Generic;

namespace Engine.Entities
{
    public class MergePreview
    {
        /// <summary>
        /// Moved from where to where - full filepaths.
        /// </summary>
        public readonly IEnumerable<(string source, string destination)> FilesToMove;

        /// <summary>
        /// Moved from where to where - full folder paths
        /// </summary>
        public readonly IEnumerable<(string source, string destination)> FoldersToMove;

        /// <summary>
        /// Files deleted - full filepaths.
        /// </summary>
        public readonly IEnumerable<string> DuplicatesToDelete;

        /// <summary>
        /// Moving files between drives is not supported
        /// </summary>
        public readonly bool HasInvalidFileMoves;

        /// <summary>
        /// Moving folders between drives is not supported
        /// </summary>
        public readonly bool HasInvalidFolderMoves;

        internal MergePreview(IEnumerable<(string source, string destination)> filesToMove, IEnumerable<(string source, string destination)> foldersToMove, IEnumerable<string> toDelete, bool hasInvalidFileMoves, bool hasInvalidFolderMoves)
        {
            FilesToMove = filesToMove;
            FoldersToMove = foldersToMove;
            DuplicatesToDelete = toDelete;
            HasInvalidFileMoves = hasInvalidFileMoves;
            HasInvalidFolderMoves = hasInvalidFolderMoves;
        }
    }
}
