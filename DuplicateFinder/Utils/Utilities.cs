using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DuplicateFinder
{
    public static class Utilities
    {
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

        public static string AppPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
