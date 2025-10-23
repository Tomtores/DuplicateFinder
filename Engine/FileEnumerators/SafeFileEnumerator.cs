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
    internal class SafeFileEnumerator : IFileEnumerator
    {
        public const int MAX_PATH = 248;
        public const int MAX_FILENAME = 260;
        public IEnumerable<string> EnumerateFiles(string[] paths, string filter)
        {
            var result = Enumerable.Empty<string>();

            foreach (var path in paths)
            {
                result = result.Concat(EnumerateFiles(path, filter));
            }

            return result.Distinct();
        }

        private IEnumerable<string> EnumerateFiles(string path, string filter)
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
                var files = Directory.EnumerateFiles(safePath, filter, SearchOption.TopDirectoryOnly);
                result = result.Concat(files);
            }
            catch (UnauthorizedAccessException)
            {
                // Unable to list folder files, skip them.
            }
           
            try
            {
                var dirs = Directory.EnumerateDirectories(safePath, "*.*", SearchOption.TopDirectoryOnly);
                result = result.Concat(dirs.SelectMany(d => EnumerateFiles(d, filter)));
            }
            catch (UnauthorizedAccessException)
            {
                // Unable to list subdirs, so skip them.
                // This should support traversal scenario.
            }
            
            return result;
        }

        
    }
}
