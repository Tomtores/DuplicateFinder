using Components;
using DuplicateFinder.Utils;
using Engine.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFinder.Forms
{
    public partial class MergeForm : Form
    {
        public bool SkipSubfolders { get; private set; } = false;
        private bool hasInvalidFileMoves;
        private bool hasInvalidFolderMoves;

        public MergeForm(MergePreview data)
        {
            InitializeComponent();
            AddItems(this.filesList, data.FilesToMove);
            AddItems(this.subfoldersList, data.FoldersToMove);
            AddItems(this.duplicatesList, data.DuplicatesToDelete);
            this.hasInvalidFileMoves = data.HasInvalidFileMoves;
            this.hasInvalidFolderMoves = data.HasInvalidFolderMoves;

            CalculateButtonState();
        }

        private void CalculateButtonState()
        {
            var cannotCommit = this.hasInvalidFileMoves || (SkipSubfolders == false && this.hasInvalidFolderMoves);
            this.CommitBtn.Enabled = !cannotCommit;
        }

        private void AddItems(ListBox itemList, IEnumerable<string> duplicatesToDelete)
        {
            var orderedItems = duplicatesToDelete.OrderBy(f => f);

            foreach (var item in orderedItems)
            {
                itemList.Items.Add($"\u274C {item}");
            }            
        }

        private void AddItems(ListBox itemList, IEnumerable<(string source, string destination)> filesToMove)
        {
            var orderedItems = filesToMove.OrderBy(f => f.source);

            foreach (var item in orderedItems)
            {
                itemList.Items.Add($"{item.source}\t-->\t{item.destination}");
            }

            if (itemList.Items.Count == 0)
            {
                itemList.Items.Add("Empty");
            }
        }

        private void skipFolderMove_CheckedChanged(object sender, EventArgs e)
        {
            if (skipFolderMove.Checked)
            {
                this.SkipSubfolders = true;
                this.subfoldersList.Enabled = false;
                this.subfoldersList.BackColor = System.Drawing.SystemColors.ButtonFace;
            }
            else
            {
                this.SkipSubfolders = false;
                this.subfoldersList.Enabled = true;
                this.subfoldersList.BackColor = System.Drawing.SystemColors.Window;
            }

            CalculateButtonState();
        }

        private void CommitBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void filesList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.filesList.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var file = filesList.Items[index].ToString().Split('\t')[0];

                var extension = Path.GetExtension(file);
                if (SecurityTool.IsExecutable(extension))
                {
                    var result = MessageBox.Show(this,
                        "You are trying to open an executable file.\nExecutable files can harm your computer.\n\nPress 'OK' to open containing folder instead, press 'Cancel' to abandon action",
                        "Insecure action prevented",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Stop);
                    if (result == DialogResult.OK)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Path.GetDirectoryName(file),
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                }
                else
                {
                    Process.Start(file);
                }
            }
        }

        private void filesList_MouseDown(object sender, MouseEventArgs e)
        {
            int index = this.filesList.IndexFromPoint(e.Location);
            
            if (e.Button == MouseButtons.Right && index != ListBox.NoMatches)
            {
                var file = filesList.Items[index].ToString().Split('\t')[0];
                
                var path = Path.GetDirectoryName(file);

                this.contextMenuStrip1.Items.Clear();
                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Open containing folder", null,
                    (send, evt) => this.OpenFolder(path)));

                this.contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void OpenFolder(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void subfoldersList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.subfoldersList.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var folder = subfoldersList.Items[index].ToString().Split('\t')[0];
                OpenFolder(folder);
            }
        }
    }
}
