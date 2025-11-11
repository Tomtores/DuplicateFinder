using Components;
using DuplicateFinder.Commands;
using DuplicateFinder.Configuration;
using DuplicateFinder.Enums;
using DuplicateFinder.Forms;
using DuplicateFinder.Utils;
using Engine;
using Engine.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreadState = System.Threading.ThreadState;
using Timer = System.Windows.Forms.Timer;

namespace DuplicateFinder
{
    public partial class DuplicateFinder : Form
    {
        private FinderSettings settings = FinderSettings.GetInstance();

        private IFinder finder;

        private ColumnNames sortColumn = ColumnNames.None;
        private bool sortAscending = false;
        private FolderFileCountCache folderFileCountCache = FolderFileCountCache.GetInstance();
        private Thread WorkerThread;
        private bool isPaused;

        private List<string> _paths = new List<string>();
        private List<string> paths
        {
            get { return _paths; }
            set
            {
                _paths = value.EmptyIfNull().ToList();
                this.settings.ScanPaths = _paths;
                this.pathBox.Text = _paths?.Count() > 1 ? "MULTIPLE FOLDERS" : _paths.FirstOrDefault();
            }
        }

        #region Change Tracking

        private bool isRunning = false;
        public ActionPendingGuard guard;
        private readonly Timer timer;
        private bool itemsChanged;  //prevent items being updated unnecessarily
        private bool statusBarChanged;  //prevent status bar updates when not needed

        private CancellationTokenSource deleteFilesCancellationToken;

        private void StartTimer()
        {
            this.timer.Enabled = true;
            this.itemsChanged = true;
            this.statusBarChanged = true;
        }

        private void ProgressUpdateCallback(ProgressKind kind)
        {
            if (kind.HasFlag(ProgressKind.StatusMessage))
            {
                this.statusBarChanged = true;
            }
            if (kind.HasFlag(ProgressKind.ItemList))
            {
                this.itemsChanged = true;
            }
        }

        #endregion

        private IEnumerable<string> deletionList = Enumerable.Empty<string>();

        /// <summary>
        /// Currently applied filter.
        /// </summary>
        protected Dictionary<FilterType, Func<IEnumerable<DuplicateViewItem[]>, IEnumerable<DuplicateViewItem[]>>> FilterActions = new Dictionary<FilterType, Func<IEnumerable<DuplicateViewItem[]>, IEnumerable<DuplicateViewItem[]>>>();

        /// <summary>
        /// Results by current filter.
        /// </summary>
        protected IEnumerable<DuplicateViewItem[]> FilteredResults
        {
            get
            {
                var result = (this.finder?.Duplicates).EmptyIfNull();
                var viewResult = BuildViewItems(result);
                foreach (var action in this.FilterActions.ToList())
                {
                    viewResult = action.Value.Invoke(viewResult);
                }
                return viewResult;
            }
        }

        private static IEnumerable<DuplicateViewItem[]> BuildViewItems(IEnumerable<Duplicate[]> result)
        {
            var directoryCounts = result.SelectMany(r => r).GroupBy(r => r.DirectoryName).ToDictionary(g => g.Key, c => c.Count());

            return result.Select(i => i.Select(el => new DuplicateViewItem(el, directoryCounts[el.DirectoryName])).ToArray());
        }

        public DuplicateFinder()
        {
            this.InitializeComponent();
            ConfigureListView(this.isRunning);

            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;

            this.timer = new Timer { Interval = 100 };
            this.timer.Tick += this.timer1_Tick;

            this.paths = this.settings.ScanPaths;
            this.ignoreBox.Text = this.settings.Ignores;

            this.trashList.Configure(() => this.settings.KeepTrashLastDir, path => this.settings.KeepTrashLastDir = path);
            this.keepList.Configure(() => this.settings.KeepTrashLastDir, path => this.settings.KeepTrashLastDir = path);

            this.guard = new ActionPendingGuard(this.browseBtn, this.browse_many_btn, this.startBtn, this.trashList, this.mergeBtn,
                this.nukeMarkedButton, this.keepList, this.menuStrip1, this.resultsView1);
        }

        private void ConfigureListView(bool disablePreview)
        {
            this.resultsView1.Configure(
                settings.CountDirectoryFiles,
                this.DeleteItems,
                this.UpdateSorting,
                this.RenderContextMenu,
                this.RenderHeaderContextMenu,
                settings.Thumbsize,
                !disablePreview && settings.PreviewEnabled
                );
        }

        private void RenderContextMenu(DuplicateViewItem item)
        {
            var path = item.DirectoryName;
            this.contextMenuStrip1.Items.Clear();
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Add to trashlist", null,
                (send, evt) => this.AddToTrashList(path)));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Add to keeplist", null,
                (send, evt) => this.AddToKeepList(path)));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Open containing folder", null,
                (send, evt) => this.OpenFolder(path)));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Merge ALL Here", null,
                async (send, evt) => this.MergeFolderAction(path)));

            this.contextMenuStrip1.Show(Cursor.Position);
        }

        private void RenderHeaderContextMenu(DuplicateViewItemHeader item)
        {
            var paths = this.finder.Duplicates
                .Where(d => d.Any(i => i.Footprint == item.Footprint))
                .SelectMany(d => d)
                .Select(d => d.DirectoryName)
                .Distinct();

            this.contextMenuStrip1.Items.Clear();
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("Open ALL folders", null,
                (send, evt) => this.OpenFolders(paths)));
            this.contextMenuStrip1.Show(Cursor.Position);
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

        private void OpenFolders(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                this.OpenFolder(path);
                Thread.Sleep(100);  // to prevent folder spam
            }
        }

        private async Task MergeFolderAction(string path)
        {
            var preview = this.finder.CalculateMergeIntoFolder(path);
            var dialog = new MergeForm(preview);
            var dialogResult = dialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                await ExecuteMerge(preview, dialog.SkipSubfolders == false);

                this.itemsChanged = true;
                this.RefreshView();
            }
        }

        #region Stage 1 - search settings

        private void browseBtn_Click(object sender, EventArgs e)
        {
            var path = Extensions.ShowFolderDialog(this.settings.LastDir);

            if (path != null)
            {
                this.settings.LastDir = path;
                this.paths = new List<string>() { path };
            }
        }

        private void browse_many_btn_Click(object sender, EventArgs e)
        {
            var foldersForm = new MultifolderSelect(() => this.settings.LastDir, newDir => this.settings.LastDir = newDir, this.paths);
            foldersForm.StartPosition = FormStartPosition.CenterParent;
            var result = foldersForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.paths = foldersForm.Result.ToList();
            }
        }

        private void ignoreBox_TextChanged(object sender, EventArgs e)
        {
            this.settings.Ignores = this.ignoreBox.Text;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            this.isRunning = true;
            this.ToggleButtons();
            Application.DoEvents();

            if (!paths.Any() || paths.Any(p => !Directory.Exists(p)))
            {
                MessageBox.Show(this, "Scan folder does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.isRunning = false;
                this.ToggleButtons();
                return;
            }

            if (!this.backgroundWorker1.IsBusy)
            {
                this.folderFileCountCache.ClearCache();  // wipe the cache when starting anew

                this.finder = FinderConfigurator.GetFinder(this.settings);

                this.deletionList = Enumerable.Empty<string>();

                // clear the view
                this.RefreshView();

                StartTimer();
                var ignored = this.ignoreBox.Text.Split(new string[] { }, StringSplitOptions.RemoveEmptyEntries);
                this.backgroundWorker1.RunWorkerAsync(new FindParameters(paths.ToArray(), ignored, settings));  //todo remove this
            }
            else
            {
                this.isRunning = false;
                this.ToggleButtons();
            }
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            if (this.isPaused)
            {
                this.WorkerThread.Resume();
                this.isPaused = false;
                this.pauseBtn.Text = "Pause";
            }
            else
            {
                this.WorkerThread.Suspend();
                this.isPaused = true;
                this.pauseBtn.Text = "Resume";
            }
        }

        private void ToggleButtons()
        {
            this.ConfigureListView(this.isRunning);
            this.startBtn.Enabled = !isRunning;
            this.pauseBtn.Enabled = isRunning;
            this.cancelBtn.Enabled = isRunning;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.deleteFilesCancellationToken?.Cancel();

            if (this.WorkerThread.ThreadState == ThreadState.Suspended)
            {
                this.WorkerThread.Resume();
            }

            this.WorkerThread.Abort();
            this.timer.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs arguments)
        {
            if (arguments.Argument is MarkTrashAction)
            {
                this.WorkerThread = new Thread(obj =>
                {
                    // todo use the marktrashaction properties??
                    var deletions = this.finder.CalculateFilesToDelete(this.trashList.Items.OfType<string>().ToList(), this.keepList.Items.OfType<string>().ToList());  //todo change into strategy
                    this.MarkForDeletion(deletions);
                    var afterDelete = this.finder.Duplicates.Count() - deletions.Count(); //todo: may be wrong count if deletions contain items from same group. 
                    var message = string.Format("{0} merge conflicts left", afterDelete);

                    this.Invoke((Action)(() =>
                    {
                        UpdateStatusStrip(message);
                    }));
                }
                );
            }
            else // do duplicate search
            {
                this.WorkerThread = new Thread(obj =>
                    {
                        var parameters = obj as FindParameters;
                        try
                        {
                            this.finder.FindDuplicates(parameters.Paths, "*.*", parameters.Ignored, ProgressUpdateCallback, parameters.Settings.IgnoreEmpty, parameters.Settings.MinSizeKB, parameters.Settings.MaxSizeKB);
                        }
                        catch (PathTooLongException exception)  //Using msgbox within thread looks evil
                        {
                            var result = MessageBox.Show("File path too long:\n'" + exception.Message + "'\n\nDo you want to open the containing folder?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                            if (result == DialogResult.Yes)
                            {
                                var path = exception.Message.TrimEnd(Path.DirectorySeparatorChar);
                                var parent = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
                                Process.Start(parent);
                            }
                        }
                        catch (FileNotFoundException exception)
                        {
                            MessageBox.Show("Could not access:\n'" + exception.Message + "'\n\nPlease ensure that file still exists and has a valid name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
            }

            this.WorkerThread.Start(arguments.Argument);

            this.WorkerThread.Join();
            try
            {
                FinderConfigurator.FlushCache();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (this.WorkerThread.ThreadState == ThreadState.Aborted)
            {
                MessageBox.Show("Task cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.timer.Enabled = false;
            this.isRunning = false;
            this.ToggleButtons();
            this.UpdateStatusStrip();
            this.RefreshView();
        }

        private void timer1_Tick(object sender, EventArgs eventArgs)
        {
            if (this.statusBarChanged)
            {
                this.statusBarChanged = false;
                this.UpdateStatusStrip();
            }

            if (this.itemsChanged)
            {
                this.itemsChanged = false;
                this.RefreshView();
            }
        }

        private void UpdateStatusStrip(string statusMessageOverride = null)
        {
            var status = this.finder.Status;

            var locale = new NumberFormatInfo() { NumberGroupSeparator = " " };

            this.progressBar.Value = status.Progress;
            var dupeCount = this.finder.Duplicates.Sum(r => r.Length);
            var fileStatusFragment = $"{status.FilesFound.ToString("N0", locale)} files, {dupeCount.ToString("N0", locale)} dupes";
            var dupeSize = this.finder.Duplicates.Sum(r => r.Sum(i => i.Size));
            var dupeSizeFragment = (dupeSize / (1024 * 1024)).ToString("N0", locale) + " MB";
            var groupStatusFragment = $"{this.finder.Duplicates.Count().ToString("N0", locale)} groups";

            string statusMessage;
            switch (status.State)
            {
                case WorkState.ListingFiles:
                case WorkState.Comparing:
                case WorkState.Deleting:
                case WorkState.Marking:
                    statusMessage = string.Format("{0}: {1}", status.State.ToString(), status.Message);
                    break;
                default:
                    statusMessage = status.State.ToString();
                    break;
            }

            this.filesStatus.Text = $"{fileStatusFragment}, {groupStatusFragment}, {dupeSizeFragment} | {statusMessageOverride ?? statusMessage}";
            this.filesStatus.ToolTipText = statusMessageOverride ?? statusMessage;

            this.statusStrip1.Refresh();

            UpdateButtonState(status);
        }

        private void UpdateButtonState(IProgressStatus status)
        {
            if (status.State == WorkState.Iddle || status.State == WorkState.Done || status.State == WorkState.Error)
            {
                this.guard.NotifyAction(isWorking: false);
            }
            else
            {
                this.guard.NotifyAction(isWorking: true);
            }
        }

        #endregion

        #region stage 2 - Listview

        /// <summary>
        /// Deletes selected items from disk and from results collection.
        /// </summary>
        private async Task DeleteItems(IEnumerable<string> deletionItems)
        {
            if (this.finder == null)
            {
                return;
            }

            this.deleteFilesCancellationToken = new CancellationTokenSource();
            this.isRunning = true;
            this.ToggleButtons();
            this.RefreshView();

            await Task.Run(async () =>
            {
                var progress = new Progress<(int total, int processed, string currentFile)>(p =>
                {
                    var percent = p.total == 0 ? 0 : (int)((p.processed / (double)p.total) * 100);
                    this.Invoke((Action)(() =>
                    {
                        this.progressBar.Value = percent;
                        this.filesStatus.Text = $"Deleting files: {p.processed} of {p.total} - {p.currentFile}";
                    }));
                });

                var errors = await this.finder.DeleteItemsAsync(deletionItems, progress, this.deleteFilesCancellationToken.Token);

                if (errors.Any())
                {
                    MessageBox.Show("There were errors when deleting: \n" + string.Join("\n", errors.Select(e => e.ToString())),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                FinderConfigurator.RemoveFromCache(deletionItems);
                foreach (var item in deletionItems)
                {
                    this.folderFileCountCache.NotifyItemRemoved(Path.GetDirectoryName(item));
                }

                // update deletion list - drop items that were already deleted
                this.deletionList = this.deletionList.Where(i => this.finder.Duplicates.SelectMany(d => d).Any(dupe => dupe.FullName == i)).ToList();
                this.deleteFilesCancellationToken = null;
            });

            this.isRunning = false;
            this.ToggleButtons();
            this.UpdateStatusStrip();
            this.RefreshView();
        }

        private async Task ExecuteMerge(MergePreview preview, bool moveSubfolders)
        {
            if (this.finder == null)
            {
                return;
            }

            this.deleteFilesCancellationToken = new CancellationTokenSource();
            this.isRunning = true;
            this.ToggleButtons();
            this.RefreshView();

            await Task.Run(async () =>
            {
                var progress = new Progress<(int total, int processed, string currentFile)>(p =>
                {
                    var percent = p.total == 0 ? 0 : (int)((p.processed / (double)p.total) * 100);
                    this.Invoke((Action)(() =>
                    {
                        this.progressBar.Value = percent;
                        this.filesStatus.Text = $"Merging folders: {p.processed} of {p.total} - {p.currentFile}";
                    }));
                });

                var errors = await this.finder.MergeIntoFolderAsync(preview, moveSubfolders, progress, this.deleteFilesCancellationToken.Token);

                if (errors.Any())
                {
                    MessageBox.Show("There were errors when processing folders: \n" + string.Join("\n", errors.Select(e => e.ToString())),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                FinderConfigurator.RemoveFromCache(preview.DuplicatesToDelete);
                foreach (var item in preview.DuplicatesToDelete)
                {
                    this.folderFileCountCache.NotifyItemRemoved(Path.GetDirectoryName(item));
                }

                // update deletion list - drop items that were already deleted
                this.deletionList = this.deletionList.Where(i => this.finder.Duplicates.SelectMany(d => d).Any(dupe => dupe.FullName == i)).ToList();
                this.deleteFilesCancellationToken = null;
            });

            this.isRunning = false;
            this.ToggleButtons();
            this.UpdateStatusStrip();
            this.RefreshView();
        }

        private void UpdateSorting(ColumnNames column)
        {
            this.FilterActions.Remove(FilterType.Sort);
            this.sortAscending = this.sortColumn == column && !this.sortAscending;
            this.sortColumn = column;

            this.FilterActions.Add(FilterType.Sort, results => SortRecordsFilter.Execute(results, column, this.sortAscending));
            RefreshView();
        }

        #endregion

        #region stage 3 - Merge

        private void AddToTrashList(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && !this.trashList.Items.Contains(path))
            {
                this.trashList.AddItem(path);
            }
        }

        private void AddToKeepList(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && !this.keepList.Items.Contains(path))
            {
                this.keepList.AddItem(path);
            }
        }

        //Calculate merge conflicts
        private void mergeBtn_Click(object sender, EventArgs e)
        {
            if (this.finder == null)
            {
                return;
            }

            //todo move to worker thread

            this.isRunning = true;
            this.ToggleButtons();

            // clear the view
            this.RefreshView();

            StartTimer();
            this.backgroundWorker1.RunWorkerAsync(new MarkTrashAction(this.trashList.Items.ToList(), this.keepList.Items.ToList()));
        }

        private void MarkForDeletion(IEnumerable<string> deletions)
        {
            this.deletionList = deletions;
            this.FilterActions.Remove(FilterType.Sort);
            this.FilterActions.Add(FilterType.Sort, results => this.CalculateMarkedOrdering(results, this.deletionList.ToList()));
            //this.resultsView1.ResetPosition();    // why is this commented?
        }

        private IEnumerable<DuplicateViewItem[]> CalculateMarkedOrdering(IEnumerable<DuplicateViewItem[]> results, IEnumerable<string> deletions)
        {
            var grouped = results.GroupBy(r => r.Any(i => deletions.Any(d => d == i.FullName)));
            var red = grouped.SingleOrDefault(g => g.Key);
            if (red == null || !red.Any())
            {
                return results;
            }

            var black = grouped.SingleOrDefault(g => !g.Key);
            var sorted = SortRecordsFilter.Execute(red.ToArray(), ColumnNames.None, true);
            if (black != null)
            {
                sorted = sorted.Concat(SortRecordsFilter.Execute(black.ToArray(), ColumnNames.None, true));
            }

            return sorted;
        }

        private async void nukeMarkedButton_Click(object sender, EventArgs e)
        {
            await this.DeleteItems(this.deletionList);
            this.resultsView1.ResetPosition();
            this.sortAscending = !this.sortAscending; //hack: to keep old sorting
            this.UpdateSorting(this.sortColumn);
        }

        #endregion

        private void configPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var config = new ConfigPanel(this.settings);
            config.StartPosition = FormStartPosition.CenterParent;
            var result = config.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.ConfigureListView(this.isRunning);
            }
        }

        private void markExtraCopiesWithinFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.finder == null)
            {
                return;
            }

            var strategy = FinderFactory.GetFolderDeduplicatorStrategy();
            var trash = strategy.MarkTrash(this.finder.Duplicates);
            this.MarkForDeletion(trash);
            this.RefreshView();
        }

        public void RefreshView()
        {
            this.ConfigureListView(this.isRunning);
            this.resultsView1.DeletionList = this.deletionList;
            this.resultsView1.Items = this.FilteredResults;
        }
    }
}