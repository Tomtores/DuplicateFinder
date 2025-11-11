using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine
{
    internal static class Extensions
    {
        internal static IEnumerable<T> NullSafe<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        internal static T GetValueSafe<T>(this T[] array, int index)
        {
            if (array.Length > index)
            {
                return array[index];
            }

            return default(T);
        }

        /// <summary>
        /// Adds an directory separator at end of the path, if not present already.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string AddDirSeparator(this string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString()) ? path : path + Path.DirectorySeparatorChar;
        }

        internal static bool AreOnSameDrive(string pathA, string PathB)
        { 
            var rootA = Path.GetPathRoot(pathA);
            var rootB = Path.GetPathRoot(PathB);
            return string.Equals(rootA, rootB, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
