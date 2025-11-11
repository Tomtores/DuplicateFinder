using Engine;
using Engine.FileEnumerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    public class FakeFileAccessor : IFileAccessor
    {
        private IDictionary<string, byte[]> _files = new Dictionary<string, byte[]>();
        private List<string> _deletedFiles = new List<string>();
        private IDictionary<string, string[]> _directories = new Dictionary<string, string[]>();

        public FakeFileAccessor()
        {            
        }

        public void SetupFiles(params (string filepath, byte[] content)[] files)
        {
            _files = files.ToDictionary(k => k.filepath, v => v.content);
        }

        public void SetupDirectories(params (string parentDirectory, string[] subdirectoryNames)[] directories)
        {
            _directories = directories.ToDictionary(d => d.parentDirectory, v => v.subdirectoryNames.Select(s => Path.Combine(v.parentDirectory, s)).ToArray());
        }

        public IEnumerable<string> EnumerateFiles(string[] paths, string filter, bool recursive)
        {
            if (recursive)
            {
                return _files.Keys.Where(k => paths.Any(p => k.StartsWith(p)));
            }

            return _files.Keys.Where(k => paths.Any(p => Path.GetDirectoryName(k).AddDirSeparator() == p.AddDirSeparator()));
        }

        public void DeleteFile(string item)
        {
            _files.Remove(item);
            _deletedFiles.Add(item);
        }

        public (string FullName, long Length, DateTime LastWriteTimeUtc) GetFileInfo(string file)
        {
            return (file, _files[file].LongLength, new DateTime(2000, 4, 1));
        }

        public byte[] GetFile(string file)
        {
            return _files[file];
        }

        internal bool VerifyDeleted(string file)
        {
            return _deletedFiles.Contains(file);
        }

        public IEnumerable<string> EnumerateDirectories(string folderPath)
        {
            return _directories[folderPath];
        }

        public void MoveFile(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public void MoveDirectory(string source, string destination)
        {
            throw new NotImplementedException();
        }
    }
}