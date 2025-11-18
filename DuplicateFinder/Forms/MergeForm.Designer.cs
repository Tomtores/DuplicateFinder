namespace DuplicateFinder.Forms
{
    partial class MergeForm
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
            this.FileMoveHeader = new System.Windows.Forms.Label();
            this.filesList = new System.Windows.Forms.ListBox();
            this.foldersLabel = new System.Windows.Forms.Label();
            this.skipFolderMove = new System.Windows.Forms.CheckBox();
            this.subfoldersList = new System.Windows.Forms.ListBox();
            this.duplicateheader = new System.Windows.Forms.Label();
            this.duplicatesList = new System.Windows.Forms.ListBox();
            this.CommitBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.fileWarninglabel = new System.Windows.Forms.Label();
            this.subfolderWarningText = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileMoveHeader
            // 
            this.FileMoveHeader.AutoSize = true;
            this.FileMoveHeader.Location = new System.Drawing.Point(3, 0);
            this.FileMoveHeader.Name = "FileMoveHeader";
            this.FileMoveHeader.Size = new System.Drawing.Size(90, 13);
            this.FileMoveHeader.TabIndex = 0;
            this.FileMoveHeader.Text = "Files to be moved";
            // 
            // filesList
            // 
            this.filesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesList.FormattingEnabled = true;
            this.filesList.Location = new System.Drawing.Point(3, 16);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(1262, 186);
            this.filesList.TabIndex = 1;
            this.filesList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.filesList_MouseDoubleClick);
            this.filesList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.filesList_MouseDown);
            // 
            // foldersLabel
            // 
            this.foldersLabel.AutoSize = true;
            this.foldersLabel.Location = new System.Drawing.Point(3, 0);
            this.foldersLabel.Name = "foldersLabel";
            this.foldersLabel.Size = new System.Drawing.Size(119, 13);
            this.foldersLabel.TabIndex = 2;
            this.foldersLabel.Text = "Subfolders to be moved";
            // 
            // skipFolderMove
            // 
            this.skipFolderMove.AutoSize = true;
            this.skipFolderMove.Location = new System.Drawing.Point(128, -1);
            this.skipFolderMove.Name = "skipFolderMove";
            this.skipFolderMove.Size = new System.Drawing.Size(138, 17);
            this.skipFolderMove.TabIndex = 3;
            this.skipFolderMove.Text = "Do not move subfolders";
            this.skipFolderMove.UseVisualStyleBackColor = true;
            this.skipFolderMove.CheckedChanged += new System.EventHandler(this.skipFolderMove_CheckedChanged);
            // 
            // subfoldersList
            // 
            this.subfoldersList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subfoldersList.FormattingEnabled = true;
            this.subfoldersList.Location = new System.Drawing.Point(6, 18);
            this.subfoldersList.Name = "subfoldersList";
            this.subfoldersList.Size = new System.Drawing.Size(1259, 186);
            this.subfoldersList.TabIndex = 4;
            this.subfoldersList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.subfoldersList_MouseDoubleClick);
            // 
            // duplicateheader
            // 
            this.duplicateheader.AutoSize = true;
            this.duplicateheader.Location = new System.Drawing.Point(3, 0);
            this.duplicateheader.Name = "duplicateheader";
            this.duplicateheader.Size = new System.Drawing.Size(107, 13);
            this.duplicateheader.TabIndex = 5;
            this.duplicateheader.Text = "Duplicates to remove";
            // 
            // duplicatesList
            // 
            this.duplicatesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.duplicatesList.FormattingEnabled = true;
            this.duplicatesList.Location = new System.Drawing.Point(6, 16);
            this.duplicatesList.Name = "duplicatesList";
            this.duplicatesList.Size = new System.Drawing.Size(1259, 199);
            this.duplicatesList.TabIndex = 6;
            // 
            // CommitBtn
            // 
            this.CommitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CommitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommitBtn.Location = new System.Drawing.Point(535, 680);
            this.CommitBtn.Name = "CommitBtn";
            this.CommitBtn.Size = new System.Drawing.Size(89, 58);
            this.CommitBtn.TabIndex = 7;
            this.CommitBtn.Text = "MERGE!";
            this.CommitBtn.UseVisualStyleBackColor = true;
            this.CommitBtn.Click += new System.EventHandler(this.CommitBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelBtn.Location = new System.Drawing.Point(651, 699);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 8;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // fileWarninglabel
            // 
            this.fileWarninglabel.AutoSize = true;
            this.fileWarninglabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileWarninglabel.ForeColor = System.Drawing.Color.Red;
            this.fileWarninglabel.Location = new System.Drawing.Point(398, 0);
            this.fileWarninglabel.Name = "fileWarninglabel";
            this.fileWarninglabel.Size = new System.Drawing.Size(353, 17);
            this.fileWarninglabel.TabIndex = 9;
            this.fileWarninglabel.Text = "Program cannot move files between disk drives!";
            this.fileWarninglabel.Visible = false;
            // 
            // subfolderWarningText
            // 
            this.subfolderWarningText.AutoSize = true;
            this.subfolderWarningText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subfolderWarningText.ForeColor = System.Drawing.Color.Red;
            this.subfolderWarningText.Location = new System.Drawing.Point(356, -2);
            this.subfolderWarningText.Name = "subfolderWarningText";
            this.subfolderWarningText.Size = new System.Drawing.Size(365, 17);
            this.subfolderWarningText.TabIndex = 10;
            this.subfolderWarningText.Text = "Program cannot move subfolders between drives!";
            this.subfolderWarningText.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1274, 662);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.duplicateheader);
            this.panel1.Controls.Add(this.duplicatesList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 443);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1268, 216);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.foldersLabel);
            this.panel2.Controls.Add(this.subfolderWarningText);
            this.panel2.Controls.Add(this.skipFolderMove);
            this.panel2.Controls.Add(this.subfoldersList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 223);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1268, 214);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.FileMoveHeader);
            this.panel3.Controls.Add(this.fileWarninglabel);
            this.panel3.Controls.Add(this.filesList);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1268, 214);
            this.panel3.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MergeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 750);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.CommitBtn);
            this.Name = "MergeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Merge Folders";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label FileMoveHeader;
        private System.Windows.Forms.ListBox filesList;
        private System.Windows.Forms.Label foldersLabel;
        private System.Windows.Forms.CheckBox skipFolderMove;
        private System.Windows.Forms.ListBox subfoldersList;
        private System.Windows.Forms.Label duplicateheader;
        private System.Windows.Forms.ListBox duplicatesList;
        private System.Windows.Forms.Button CommitBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label fileWarninglabel;
        private System.Windows.Forms.Label subfolderWarningText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}