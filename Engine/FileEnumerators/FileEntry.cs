using System;

namespace Engine.FileEnumerators
{
    public readonly struct FileEntry
    {
        public string FullName { get; }
        public long Size { get; }
        public DateTime LastWriteTimeUtc { get; }

        public FileEntry(string fullName, long size, DateTime lastWriteTimeUtc)
        {
            FullName = fullName;
            Size = size;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }
    }
}
