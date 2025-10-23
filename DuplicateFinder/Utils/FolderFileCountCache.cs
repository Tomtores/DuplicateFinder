using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder
{
    // maintains cached count of folder files.
    internal class FolderFileCountCache
    {
        private Dictionary<string, int> folderFileCountCache = new Dictionary<string, int>();

        public void ClearCache()
        {
            this.folderFileCountCache.Clear();
        }

        //must be a directory path
        public int GetDirectoryFileCount(string path)
        {
            if (!this.folderFileCountCache.ContainsKey(path))
            {
                this.folderFileCountCache[path] = CountDirectoryFiles(path);
            }

            return this.folderFileCountCache[path];
        }

        private static int CountDirectoryFiles(string path)
        {
            try
            {
                var dir = new DirectoryInfo(path);
                return dir.EnumerateFiles().Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //must be a directory path
        public void NotifyItemRemoved(string path)
        {
            folderFileCountCache.Remove(path); //safe to call even if item does not exist
        }

        #region Singleton
        private static FolderFileCountCache Instance;

        public static FolderFileCountCache GetInstance()
        {
            if (Instance == null)
            {
                Instance = new FolderFileCountCache();
            }

            return Instance;
        }

        private FolderFileCountCache()
        {
            
        }

        #endregion
    }
}
