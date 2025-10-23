namespace DuplicateFinder
{
    partial class DuplicateFinder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startBtn = new System.Windows.Forms.Button();
            this.browseBtn = new System.Windows.Forms.Button();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mergeBtn = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.filesStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.nukeMarkedButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.keepList = new FolderList();
            this.trashList = new FolderList();
            this.ignoreBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.resultsView1 = new Components.ResultsView();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markExtraCopiesWithinFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browse_many_btn = new System.Windows.Forms.Button();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(540, 27);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 0;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // browseBtn
            // 
            this.browseBtn.Location = new System.Drawing.Point(280, 27);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(75, 23);
            this.browseBtn.TabIndex = 1;
            this.browseBtn.Text = "Browse...";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // pathBox
            // 
            this.pathBox.Enabled = false;
            this.pathBox.Location = new System.Drawing.Point(59, 29);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(215, 20);
            this.pathBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(480, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Keep List";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Trash List:";
            // 
            // mergeBtn
            // 
            this.mergeBtn.Location = new System.Drawing.Point(304, 19);
            this.mergeBtn.Name = "mergeBtn";
            this.mergeBtn.Size = new System.Drawing.Size(86, 69);
            this.mergeBtn.TabIndex = 12;
            this.mergeBtn.Text = "Mark Trash";
            this.mergeBtn.UseVisualStyleBackColor = true;
            this.mergeBtn.Click += new System.EventHandler(this.mergeBtn_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.filesStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(823, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // filesStatus
            // 
            this.filesStatus.AutoSize = false;
            this.filesStatus.Name = "filesStatus";
            this.filesStatus.Size = new System.Drawing.Size(706, 17);
            this.filesStatus.Spring = true;
            this.filesStatus.Text = "0 files, 0 dupes, 0 groups, 0 MB | Status";
            this.filesStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nukeMarkedButton
            // 
            this.nukeMarkedButton.Location = new System.Drawing.Point(403, 19);
            this.nukeMarkedButton.Name = "nukeMarkedButton";
            this.nukeMarkedButton.Size = new System.Drawing.Size(81, 69);
            this.nukeMarkedButton.TabIndex = 15;
            this.nukeMarkedButton.Text = "Delete marked";
            this.nukeMarkedButton.UseVisualStyleBackColor = true;
            this.nukeMarkedButton.Click += new System.EventHandler(this.nukeMarkedButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.keepList);
            this.panel1.Controls.Add(this.trashList);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.nukeMarkedButton);
            this.panel1.Controls.Add(this.mergeBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(12, 379);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 130);
            this.panel1.TabIndex = 16;
            // 
            // keepList
            // 
            this.keepList.Location = new System.Drawing.Point(490, 16);
            this.keepList.Name = "keepList";
            this.keepList.Size = new System.Drawing.Size(306, 109);
            this.keepList.TabIndex = 21;
            // 
            // trashList
            // 
            this.trashList.Location = new System.Drawing.Point(3, 16);
            this.trashList.Name = "trashList";
            this.trashList.Size = new System.Drawing.Size(295, 109);
            this.trashList.TabIndex = 20;
            // 
            // ignoreBox
            // 
            this.ignoreBox.Location = new System.Drawing.Point(434, 29);
            this.ignoreBox.Name = "ignoreBox";
            this.ignoreBox.Size = new System.Drawing.Size(100, 20);
            this.ignoreBox.TabIndex = 17;
            this.ignoreBox.TextChanged += new System.EventHandler(this.ignoreBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(388, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Ignore:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Folder:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.resultsView1);
            this.panel2.Location = new System.Drawing.Point(12, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(799, 317);
            this.panel2.TabIndex = 21;
            // 
            // resultsView1
            // 
            this.resultsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsView1.Location = new System.Drawing.Point(0, 0);
            this.resultsView1.Name = "resultsView1";
            this.resultsView1.Size = new System.Drawing.Size(799, 317);
            this.resultsView1.TabIndex = 0;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Enabled = false;
            this.cancelBtn.Location = new System.Drawing.Point(702, 27);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 22;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(823, 24);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configPanelToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // configPanelToolStripMenuItem
            // 
            this.configPanelToolStripMenuItem.Name = "configPanelToolStripMenuItem";
            this.configPanelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.configPanelToolStripMenuItem.Text = "Config Panel";
            this.configPanelToolStripMenuItem.Click += new System.EventHandler(this.configPanelToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markExtraCopiesWithinFolderToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // markExtraCopiesWithinFolderToolStripMenuItem
            // 
            this.markExtraCopiesWithinFolderToolStripMenuItem.Name = "markExtraCopiesWithinFolderToolStripMenuItem";
            this.markExtraCopiesWithinFolderToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
            this.markExtraCopiesWithinFolderToolStripMenuItem.Text = "Mark extra copies within folder";
            this.markExtraCopiesWithinFolderToolStripMenuItem.Click += new System.EventHandler(this.markExtraCopiesWithinFolderToolStripMenuItem_Click);
            // 
            // browse_many_btn
            // 
            this.browse_many_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browse_many_btn.Location = new System.Drawing.Point(361, 27);
            this.browse_many_btn.Name = "browse_many_btn";
            this.browse_many_btn.Size = new System.Drawing.Size(21, 23);
            this.browse_many_btn.TabIndex = 25;
            this.browse_many_btn.Text = "+";
            this.browse_many_btn.UseVisualStyleBackColor = true;
            this.browse_many_btn.Click += new System.EventHandler(this.browse_many_btn_Click);
            // 
            // pauseBtn
            // 
            this.pauseBtn.Enabled = false;
            this.pauseBtn.Location = new System.Drawing.Point(621, 27);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(75, 23);
            this.pauseBtn.TabIndex = 26;
            this.pauseBtn.Text = "Pause";
            this.pauseBtn.UseVisualStyleBackColor = true;
            this.pauseBtn.Click += new System.EventHandler(this.pauseBtn_Click);
            // 
            // DuplicateFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 534);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.browse_many_btn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ignoreBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pathBox);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.startBtn);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DuplicateFinder";
            this.Text = "Duplicate Finder";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mergeBtn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel filesStatus;
        private System.Windows.Forms.Button nukeMarkedButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ignoreBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markExtraCopiesWithinFolderToolStripMenuItem;
        private Components.ResultsView resultsView1;
        private System.Windows.Forms.Button browse_many_btn;
        private FolderList trashList;
        private FolderList keepList;
        private System.Windows.Forms.Button pauseBtn;
    }
}

