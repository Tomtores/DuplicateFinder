using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.FileEnumerators
{
    /// <summary>
    /// Performs the built-in file enumeration.
    /// </summary>
    internal class StandardFileEnumerator : IFileAccessor
    {
        public void DeleteFile(string item)
        {
            FileSystem.DeleteFile(item, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public IEnumerable<string> EnumerateDirectories(string folderPath)
        {
            return Directory.EnumerateDirectories(folderPath);  // note: library method searches only top level directory by deafult.
        }

        public IEnumerable<string> EnumerateFiles(string[] paths, string filter, bool recursive)
        {
            var result = Enumerable.Empty<string>();

            foreach (var path in paths)
            {
                var searchMode = recursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
                result = result.Concat(Directory.EnumerateFiles(path, filter, searchMode));
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
            Directory.Move(source, destination);
        }

        public void MoveFile(string source, string destination)
        {
            File.Move(source, destination);
        }
    }
}
