using System;
using System.Collections.Generic;

namespace Engine.FileEnumerators
{
    /// <summary>
    /// Provides file access methods.
    /// </summary>
    public interface IFileAccessor
    {
        void DeleteFile(string item);

        /// <summary>
        /// Enumerates files on given path.
        /// The results are unique.
        /// </summary>
        IEnumerable<string> EnumerateFiles(string[] paths, string filter, bool recursive);

        /// <summary>
        /// Enumerates directories in given folder. Tope level only.
        /// </summary>
        IEnumerable<string> EnumerateDirectories(string folderPath);

        (string FullName, long Length, DateTime LastWriteTimeUtc) GetFileInfo(string file);
        
        void MoveFile(string source, string destination);
        void MoveDirectory(string source, string destination);
    }
}
