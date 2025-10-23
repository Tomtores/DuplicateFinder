using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.FileEnumerators
{
    /// <summary>
    /// Performs the built-in file enumeration.
    /// </summary>
    internal class StandardFileEnumerator : IFileEnumerator
    {
        public IEnumerable<string> EnumerateFiles(string[] paths, string filter)
        {
            var result = Enumerable.Empty<string>();

            foreach (var path in paths)
            {
                result = result.Concat(Directory.EnumerateFiles(path, filter, SearchOption.AllDirectories));
            }

            return result.Distinct();
        }
    }
}
