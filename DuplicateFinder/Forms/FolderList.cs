using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFinder
{
    public partial class FolderList : UserControl
    {
        public IOrderedEnumerable<string> Items { get { return this.folderListView.Items.OfType<string>().OrderBy(x => 0); } }
        private Func<string> lastDirGet;
        private Action<string> lastDirSet;
        private int sortOrder = 0;

        public void AddItem(string item)
        {
            this.folderListView.Items.Add(item);
            this.folderListView.Refresh();
        }

        public FolderList()
        {
            InitializeComponent();
            this.removeBtn.Enabled = false;
            this.addBtn.Enabled = false;
            this.clearBtn.Enabled = false;
        }
        public void Configure(Func<string> lastDirGet, Action<string> lastDirSet, IEnumerable<string> lastValue = null)
        {
            this.lastDirGet = lastDirGet;
            this.lastDirSet = lastDirSet;
            if (lastValue != null)
            {
                this.folderListView.Items.AddRange(lastValue.ToArray());
                this.folderListView.Refresh();
            }

            this.removeBtn.Enabled = true;
            this.addBtn.Enabled = true;
            this.clearBtn.Enabled = true;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            var path = Extensions.ShowFolderDialog(lastDirGet());
            if (path != null)
            {
                this.lastDirSet(path);
                this.AddToList(path);
            }
        }

        private void AddToList(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && !this.folderListView.Items.Contains(path))
            {
                this.folderListView.Items.Add(path);
            }
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            var index = this.folderListView.SelectedIndex;
            if (index > -1)
            {
                this.folderListView.Items.RemoveAt(index);
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            this.folderListView.Items.Clear();
        }

        private void sortBtn_Click(object sender, EventArgs e)
        {
            if (sortOrder <= 0)
            {
                sortOrder = 1;
            }
            else
            {
                sortOrder = -1;
            }

            var orderedItems = (sortOrder > 0 ? this.Items.OrderBy(i => i) : this.Items.OrderByDescending(i => i)).ToList();
            this.folderListView.Items.Clear();
            foreach (var item in orderedItems)
            {
                this.folderListView.Items.Add(item);
            }
            this.folderListView.Refresh();
        }
    }
}
