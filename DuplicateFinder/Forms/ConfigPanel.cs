using DuplicateFinder.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DuplicateFinder
{
    public partial class ConfigPanel : Form
    {
        private FinderSettings Settings { get; set; }

        public ConfigPanel(FinderSettings settings)
        {
            InitializeComponent();
            this.versionLabel.Text = "Version " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly‌​().Location).ProductVersion; 
            this.Settings = settings;

            //read setting values
            this.usecrcCheck.Checked = this.Settings.UseCRC;
            this.fileCountEnabledCheckbox.Checked = this.Settings.CountDirectoryFiles;
            this.hashCachingEnabledBox.Checked = this.Settings.UseHashCaching;
            this.IgnoreEmptyBox.Checked = this.Settings.IgnoreEmpty;
            this.minSizeBox.Text = this.Settings.MinSizeKB == null ? "" : this.Settings.MinSizeKB.ToString();
            this.maxSizeBox.Text = this.Settings.MaxSizeKB == null ? "" : this.Settings.MaxSizeKB.ToString();
            this.thumbnailSize.Value = Math.Min(256, Math.Max(this.Settings.Thumbsize, 12)); // Must be 12 to 256 pixels. One day in net 7. we'll get Math.Clamp()
            this.previewCheck.Checked = this.Settings.PreviewEnabled;
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            // Validate
            var minSize = GetValueOrNull(this.minSizeBox.Text);
            var maxSize = GetValueOrNull(this.maxSizeBox.Text);

            if (maxSize <= minSize)
            {
                MessageBox.Show("Minimum size cannot be greater than max size. The resulting range is empty!", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Apply new settings
            this.Settings.IgnoreEmpty = this.IgnoreEmptyBox.Checked;
            this.Settings.UseCRC = this.usecrcCheck.Checked;
            this.Settings.CountDirectoryFiles = this.fileCountEnabledCheckbox.Checked;
            this.Settings.UseHashCaching = this.hashCachingEnabledBox.Checked;

            this.Settings.MinSizeKB = minSize;
            this.Settings.MaxSizeKB = maxSize;
            this.Settings.Thumbsize = (int)Math.Round(this.thumbnailSize.Value, 0);
            this.Settings.PreviewEnabled = this.previewCheck.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private int? GetValueOrNull(string number)
        {
            int value;
            if (int.TryParse(number, out value))
            {
                return value;
            }

            return null;
        }

        private void Cancel_btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void deletCacheBtn_Click(object sender, EventArgs e)
        {
            var cachePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cache.tsv");
            if (!File.Exists(cachePath))
            {
                MessageBox.Show("No cache to delete", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Deleting cache - Are you sure?", "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    File.Delete(cachePath);
                }                
            }
        }

        private void trimCache_Click(object sender, EventArgs e)
        {
            var cachePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cache.tsv");
            if (!File.Exists(cachePath))
            {
                MessageBox.Show("No cache exists", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var dialog = new TrimCacheDialog();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog();
            }
        }

        private void hashCachingEnabledBox_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Checked == true)
            {
                var warningText = "This setting will save potentially sensitive file metadata like: full path, file size and checksum, in program folder.\n"
                    + "\nClick 'Yes' below to acknowledge you understood this warning and want to enable the cache, otherwise click 'no'";
                var result = MessageBox.Show(warningText, "CAUTION!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    checkBox.Checked = true;
                }
                else
                {
                    checkBox.Checked = false;
                }
            }
        }
    }
}
