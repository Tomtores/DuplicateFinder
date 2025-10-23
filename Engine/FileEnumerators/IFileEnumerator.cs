using System.Collections.Generic;

namespace Engine.FileEnumerators
{
    /// <summary>
    /// manages file enumeration for given path.
    /// </summary>
    public interface IFileEnumerator
    {
        /// <summary>
        /// Enumerates files on given path.
        /// The results are unique.
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<string> EnumerateFiles(string[] paths, string filter);
    }
}
