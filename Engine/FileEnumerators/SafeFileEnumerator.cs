using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Engine.FileEnumerators
{
    /// <summary>
    /// Enumerates files on given path. Will ignore folders and files that are forbidden. Will not crash on access denied.
    /// </summary>
    internal class SafeFileEnumerator : IFileAccessor
    {
        public const int MAX_PATH = 248;
        public const int MAX_FILENAME = 260;

        public void DeleteFile(string item)
        {
            if (!File.Exists(item))
            {
                throw new FileNotFoundException(item);
            }

            if (File.GetAttributes(item).HasFlag(FileAttributes.ReadOnly))
            {
                throw new UnauthorizedAccessException(item);
            }

            FileSystem.DeleteFile(item, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public IEnumerable<string> EnumerateDirectories(string folderPath)
        {
            var result = new List<string>();

            //Hack: fix issues with directories having "funny" names (like single character Alt+0160) being interpreted as whitespace and trimmed.
            var safePath = folderPath.AddDirSeparator();

            try
            {
                var dirs = Directory.EnumerateDirectories(safePath, "*", System.IO.SearchOption.TopDirectoryOnly);
                result.AddRange(dirs);
            }
            catch (UnauthorizedAccessException)
            {
                // Unable to list subdirs, so skip them.
                // This should support traversal scenario.
            }

            return result;
        }

        public IEnumerable<string> EnumerateFiles(string[] paths, string filter, bool recursive)
        {
            var result = Enumerable.Empty<string>();

            foreach (var path in paths)
            {
                result = result.Concat(EnumerateFiles(path, filter, recursive));
            }

            return result.Distinct();
        }

        public (string FullName, long Length, DateTime LastWriteTimeUtc) GetFileInfo(string file)
        {
            var info = new FileInfo(file);
            return (info.FullName, info.Length, info.LastWriteTimeUtc);
        }

        public void MoveDirectory(string source, string destination)
        {
            var sourceSafe = source.AddDirSeparator();
            var destinationSafe = destination.AddDirSeparator();

            if (!Directory.Exists(sourceSafe))
            {
                throw new DirectoryNotFoundException(source);
            }

            if (Directory.Exists(destinationSafe))
            {
                throw new IOException($"Destination directory already exists: {destination}");
            }

            if (!Extensions.AreOnSameDrive(sourceSafe, destinationSafe))
            {
                throw new InvalidOperationException("Cannot move folder to another drive");
            }

            if (string.Equals(sourceSafe, destinationSafe, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("Source and destination directories are the same");
            }

            Directory.Move(source, destination);
        }

        public void MoveFile(string source, string destination)
        {
            if (!File.Exists(source))
            {
                throw new FileNotFoundException(source);
            }

            if (File.Exists(destination))
            {
                throw new IOException($"Destination file already exists: {destination}");
            }

            if (!Extensions.AreOnSameDrive(source, destination))
            {
                throw new InvalidOperationException("Cannot move file to another drive");
            }

            if (string.Equals(source, destination, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException("Source and destination files are the same");
            }

            File.Move(source, destination);
        }

        private IEnumerable<string> EnumerateFiles(string path, string filter, bool recursive)
        {
            var result = Enumerable.Empty<string>();

            //Hack: fix issues with directories having "funny" names (like single character Alt+0160) being interpreted as whitespace and trimmed.
            var safePath = path.AddDirSeparator();

            if (safePath.Length > MAX_PATH)
            {
                throw new PathTooLongException(safePath);   // Workaround to catch overly long path. If it throws inside ConcatenateIterator, the path name is lost.
            }

            if (File.GetAttributes(safePath).HasFlag(FileAttributes.ReparsePoint))
            {
                return result;  //Skip NTFS hardlinks to avoid endless loops
            }

            if (Regex.IsMatch(safePath, @"^[A-Z]:\\\$Recycle\.Bin.*$", RegexOptions.IgnoreCase))
            {
                return result;  // do not index recycle bin
            }

            try
            {
                var files = Directory.EnumerateFiles(safePath, filter, System.IO.SearchOption.TopDirectoryOnly);
                result = result.Concat(files);
            }
            catch (UnauthorizedAccessException)
            {
                // Unable to list folder files, skip them.
            }

            if (recursive)
            {
                try
                {
                    var dirs = Directory.EnumerateDirectories(safePath, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                    result = result.Concat(dirs.SelectMany(d => EnumerateFiles(d, filter, recursive)));
                }
                catch (UnauthorizedAccessException)
                {
                    // Unable to list subdirs, so skip them.
                    // This should support traversal scenario.
                }
            }

            return result;
        }
    }
}
