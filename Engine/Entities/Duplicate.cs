using System.IO;
namespace Engine.Entities
{
    /// <summary>
    /// Holds basic info about duplicated file.
    /// </summary>
    public class Duplicate
    {
        /// <param name="fullName">Full file path (unique)</param>
        /// <param name="size">Size (in bytes)</param>
        /// <param name="directoryName">Directory path</param>
        public Duplicate(string fullName, long size)
        {
            FullName = fullName;
            Size = size;
            DirectoryName = string.IsNullOrWhiteSpace(fullName) ? fullName : Path.GetDirectoryName(fullName);
        }

        /// <summary>
        /// Full file path (Unique)
        /// </summary>
        public string FullName { get; private set; }
        public long Size { get; private set; }
        public string DirectoryName { get; private set; }

        public string Hash { get; set; }

        /// <summary>
        /// Unique value that identifies group of duplicates.
        /// </summary>
        public string Footprint { get { return this.Hash + this.Size; } }
    }
}
