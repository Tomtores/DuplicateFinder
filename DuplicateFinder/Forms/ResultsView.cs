using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Engine.Entities;
using DuplicateFinder;
using DuplicateFinder.Enums;

namespace Components
{
    /// <summary>
    /// Used to display results.
    /// </summary>
    public partial class ResultsView : UserControl
    {
        private Action<IEnumerable<string>> onDelete;
        private Action<ColumnNames> onUpdateSortColumn;
        private Action<DuplicateViewItem> onItemRightclick;
        private bool countDirectoryFiles;
        private int thumbsize;
        private IEnumerable<string> deletionList;
        private Duplicate[] flatList;
        private FolderFileCountCache folderFileCount = FolderFileCountCache.GetInstance();
        private bool showPreview;

        public IEnumerable<DuplicateViewItem[]> Items
        {
            set
            {
                var items = value.EmptyIfNull().ToList();
                this.flatList = items.SelectMany(i => GetHeader(i[0]).Concat(i)).ToArray();
                this.RefreshView();
            }
        }

        private IEnumerable<Duplicate> GetHeader(DuplicateViewItem i)
        {
            // use bold font style
            var item = new DuplicateViewItemHeader(i.Size, i.Hash, i.FullName);
            return new[] { item };
        }

        public IEnumerable<string> DeletionList
        {
            set
            {
                this.deletionList = value.EmptyIfNull().ToList();
            }
        }

        public ResultsView()
        {
            InitializeComponent();

            this.listView1.View = View.Details;
            ConfigureColumns(false);
            this.listView1.Activation = ItemActivation.TwoClick;
            this.listView1.SmallImageList = this.imageList1;

            this.listView1.ItemActivate += this.ListViewOnItemActivate;
            this.listView1.ColumnClick += this.resultsView_ColumnClick;
            this.listView1.Resize += this.resultsView_Resize;
            this.listView1.KeyDown += this.resultsView_KeyDown;
            this.listView1.MouseClick += this.listView_MouseClick;

            this.listView1.VirtualMode = true;
            this.listView1.RetrieveVirtualItem += this.RetrieveItem;
            this.listView1.FullRowSelect = true;
        }

        public void Configure(bool countDirectoryFiles, Action<IEnumerable<string>> onDelete, Action<ColumnNames> onUpdateSortColumn, Action<DuplicateViewItem> onItemRightclick, int thumbsize, bool previewEnabled)
        {
            this.onDelete = onDelete;
            this.onUpdateSortColumn = onUpdateSortColumn;
            this.onItemRightclick = onItemRightclick;
            this.countDirectoryFiles = countDirectoryFiles;
            this.thumbsize = Math.Min(256, thumbsize);
            this.showPreview = previewEnabled;
            ConfigureColumns(countDirectoryFiles);
                        
            if (showPreview)
            {
                this.imageList1.ImageSize = new Size(this.thumbsize, this.thumbsize);
            }
        }

        private void ConfigureColumns(bool countDirectoryFiles)
        {
            this.listView1.Columns.Clear();
            this.listView1.Columns.Add(ColumnNames.Path.ToString(), "Path", 700);
            this.listView1.Columns.Add(ColumnNames.Count.ToString(), "Count", 40);
            this.listView1.Columns.Add(ColumnNames.Size.ToString(), "Size", 85);
            if (countDirectoryFiles)
            {
                this.listView1.Columns.Insert(2, ColumnNames.FolderCount.ToString(), "Folder", 40);
            }
            this.SizeFirstColumn(this.listView1);
        }

        public void ResetPosition()
        {
            this.listView1.SelectedIndices.Clear();
            if (this.listView1.Items.Count > 0)
            {
                this.listView1.TopItem = this.listView1.Items[0];
            }
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            var list = sender as ListView;
            if (e.Button == MouseButtons.Right && list.SelectedIndices.Count > 0)
            {
                var index = list.SelectedIndices[0];
                var item = GetItemAtIndex(index);
                if (item is DuplicateViewItemHeader) //check if header
                {
                    // do nothing
                }
                else
                {
                    this.onItemRightclick(item as DuplicateViewItem);
                }
            }
        }

        private void ListViewOnItemActivate(object sender, EventArgs eventArgs)
        {
            if (this.listView1.SelectedIndices.Count == 1)
            {
                var index = this.listView1.SelectedIndices[0];
                var item = GetItemAtIndex(index);
                if (item is DuplicateViewItemHeader)
                {
                    // do nothing
                }
                {
                    Process.Start(item.FullName);
                }
            }
        }

        private void resultsView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.onUpdateSortColumn(GetColumnName(e.Column));
        }

        private ColumnNames GetColumnName(int column)
        {
            return (ColumnNames)Enum.Parse(typeof(ColumnNames), this.listView1.Columns[column].Name);
        }

        private void resultsView_Resize(object sender, EventArgs e)
        {
            this.SizeFirstColumn(this.listView1);
        }

        private void resultsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var selected = this.listView1.SelectedIndices;
                var pos = selected.Count > 0 ? selected[0] : 0;
                var deletionItems = new List<string>();
                foreach (int index in selected)
                {
                    var item = GetItemAtIndex(index);
                    if (item is DuplicateViewItemHeader)
                    {
                        // do nothing
                    }
                    else
                    {
                        deletionItems.Add(item.FullName);
                    }
                }

                this.onDelete(deletionItems);
            }
        }

        private void RefreshView(int scrollToIndex = -1)
        {
            this.listView1.BeginUpdate();
            this.listView1.VirtualListSize = this.flatList.Length;

            this.listView1.EndUpdate();

            var index = Math.Min(scrollToIndex, this.listView1.Items.Count - 1);
            if (index > 0)
            {
                this.listView1.EnsureVisible(index);
            }

            this.listView1.Enabled = this.Enabled;
            this.listView1.Refresh();
        }

        private void RetrieveItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var item = GetItemAtIndex(e.ItemIndex);
            e.Item = BuildViewItem(item);
        }

        private Duplicate GetItemAtIndex(int index)
        {
            var item = flatList.GetValueSafe(index);
            return item;
        }

        private ListViewItem BuildViewItem(Duplicate item)
        {
            if (item is DuplicateViewItemHeader)
            {
                var result2 = new ListViewItem("Item");
                result2.SubItems.Add("");
                result2.SubItems.Add("");
                if (this.countDirectoryFiles)
                {
                    result2.SubItems.Add("");
                }
                result2.Name = item.Footprint;
                result2.Font = new Font(result2.Font, FontStyle.Bold);
                if (showPreview)
                {
                    this.SetHeaderIcon(result2, item);
                }

                return result2;
            }

            var casted = item as DuplicateViewItem;
            var result = new ListViewItem { Text = casted.FullName, Name = casted.Footprint };

            result.SubItems.Add(casted.DirectoryDuplicatesCount.ToString());    // duplicate files count column

            if (this.countDirectoryFiles)
            {
                result.SubItems.Add(this.folderFileCount.GetDirectoryFileCount(casted.DirectoryName).ToString());   // total folder file count   
            }
            
            result.SubItems.Add(casted.Size.ToString());    // size column

            if (!showPreview)
            {
                this.SetIcon(result, casted);
            }
            if (this.deletionList.Contains(casted.FullName))
            {
                result.ForeColor = Color.Red;
            }
            return result;
        }

        private void SetIcon(ListViewItem item, Duplicate duplicate)
        {
            // Check to see if the image collection contains an image 
            // for this extension, using the extension as a key. 
            var extension = Path.GetExtension(duplicate.FullName);
            if (!this.imageList1.Images.ContainsKey(extension) && File.Exists(duplicate.FullName))
            {
                // If not, add the image to the image list.
                var iconForFile = Icon.ExtractAssociatedIcon(duplicate.FullName);
                this.imageList1.Images.Add(extension, iconForFile);
            }
            item.ImageIndex = this.imageList1.Images.IndexOfKey(extension); //In virtual mode ImageKey does not work, use image index.
        }

        private void SetHeaderIcon(ListViewItem item, Duplicate duplicate)
        {
            // Check to see if the image is already loaded 
            if (!this.imageList1.Images.ContainsKey(duplicate.FullName) && File.Exists(duplicate.FullName))
            {
                Image thumb = new Bitmap(thumbsize, thumbsize);
                try
                {
                    var image = Image.FromFile(duplicate.FullName);
                    thumb = image.GetThumbnailImage(thumbsize, thumbsize, null, IntPtr.Zero);                    
                }
                finally
                {
                    this.imageList1.Images.Add(duplicate.FullName, thumb);
                }
            }

            item.ImageIndex = this.imageList1.Images.IndexOfKey(duplicate.FullName); //In virtual mode ImageKey does not work, use image index.
        }

        private void SizeFirstColumn(ListView lv)
        {
            lv.Columns[0].Width = lv.Width - lv.Columns[1].Width - lv.Columns[2].Width - (lv.Columns.Count > 3 ? lv.Columns[3].Width : 0) - SystemInformation.VerticalScrollBarWidth - 5;
        }        
    }
}
