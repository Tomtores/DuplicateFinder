using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFinder
{
    public static class Extensions
    {
        public static T GetValueSafe<T>(this T[] array, int index)
        {
            if (array.Length > index)
            {
                return array[index];
            }

            return default(T);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> input)
        {
            return input ?? Enumerable.Empty<T>();
        }

        public static string ShowFolderDialog(string lastPath)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = lastPath;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return null;
        }
    }
}
