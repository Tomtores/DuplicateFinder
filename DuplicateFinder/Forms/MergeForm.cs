using Engine.Entities;
using System;
using System.Collections.Generic;
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
    }
}
