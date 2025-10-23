namespace DuplicateFinder
{
    partial class ConfigPanel
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
            this.usecrcCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ok_btn = new System.Windows.Forms.Button();
            this.Cancel_btn = new System.Windows.Forms.Button();
            this.IgnoreEmptyBox = new System.Windows.Forms.CheckBox();
            this.minSizeBox = new System.Windows.Forms.MaskedTextBox();
            this.maxSizeBox = new System.Windows.Forms.MaskedTextBox();
            this.fileCountEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.hashCachingEnabledBox = new System.Windows.Forms.CheckBox();
            this.previewCheck = new System.Windows.Forms.CheckBox();
            this.thumbnailSize = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.deletCacheBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.trimCache = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailSize)).BeginInit();
            this.SuspendLayout();
            // 
            // usecrcCheck
            // 
            this.usecrcCheck.AutoSize = true;
            this.usecrcCheck.Location = new System.Drawing.Point(12, 12);
            this.usecrcCheck.Name = "usecrcCheck";
            this.usecrcCheck.Size = new System.Drawing.Size(82, 17);
            this.usecrcCheck.TabIndex = 24;
            this.usecrcCheck.Text = "Use CRC32";
            this.usecrcCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "to";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "KB";
            // 
            // ok_btn
            // 
            this.ok_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok_btn.Location = new System.Drawing.Point(233, 227);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(75, 23);
            this.ok_btn.TabIndex = 30;
            this.ok_btn.Text = "OK";
            this.ok_btn.UseVisualStyleBackColor = true;
            this.ok_btn.Click += new System.EventHandler(this.ok_btn_Click);
            // 
            // Cancel_btn
            // 
            this.Cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel_btn.Location = new System.Drawing.Point(314, 227);
            this.Cancel_btn.Name = "Cancel_btn";
            this.Cancel_btn.Size = new System.Drawing.Size(75, 23);
            this.Cancel_btn.TabIndex = 31;
            this.Cancel_btn.Text = "Cancel";
            this.Cancel_btn.UseVisualStyleBackColor = true;
            this.Cancel_btn.Click += new System.EventHandler(this.Cancel_btn_Click);
            // 
            // IgnoreEmptyBox
            // 
            this.IgnoreEmptyBox.AutoSize = true;
            this.IgnoreEmptyBox.Location = new System.Drawing.Point(12, 35);
            this.IgnoreEmptyBox.Name = "IgnoreEmptyBox";
            this.IgnoreEmptyBox.Size = new System.Drawing.Size(108, 17);
            this.IgnoreEmptyBox.TabIndex = 32;
            this.IgnoreEmptyBox.Text = "Ignore empty files";
            this.IgnoreEmptyBox.UseVisualStyleBackColor = true;
            // 
            // minSizeBox
            // 
            this.minSizeBox.Location = new System.Drawing.Point(52, 122);
            this.minSizeBox.Mask = "0000000";
            this.minSizeBox.Name = "minSizeBox";
            this.minSizeBox.Size = new System.Drawing.Size(52, 20);
            this.minSizeBox.TabIndex = 33;
            // 
            // maxSizeBox
            // 
            this.maxSizeBox.Location = new System.Drawing.Point(132, 122);
            this.maxSizeBox.Mask = "000000000";
            this.maxSizeBox.Name = "maxSizeBox";
            this.maxSizeBox.Size = new System.Drawing.Size(56, 20);
            this.maxSizeBox.TabIndex = 34;
            // 
            // fileCountEnabledCheckbox
            // 
            this.fileCountEnabledCheckbox.AutoSize = true;
            this.fileCountEnabledCheckbox.Location = new System.Drawing.Point(12, 58);
            this.fileCountEnabledCheckbox.Name = "fileCountEnabledCheckbox";
            this.fileCountEnabledCheckbox.Size = new System.Drawing.Size(104, 17);
            this.fileCountEnabledCheckbox.TabIndex = 35;
            this.fileCountEnabledCheckbox.Text = "Count folder files";
            this.fileCountEnabledCheckbox.UseVisualStyleBackColor = true;
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(12, 232);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(41, 13);
            this.versionLabel.TabIndex = 36;
            this.versionLabel.Text = "version";
            // 
            // hashCachingEnabledBox
            // 
            this.hashCachingEnabledBox.AutoSize = true;
            this.hashCachingEnabledBox.Location = new System.Drawing.Point(12, 99);
            this.hashCachingEnabledBox.Name = "hashCachingEnabledBox";
            this.hashCachingEnabledBox.Size = new System.Drawing.Size(112, 17);
            this.hashCachingEnabledBox.TabIndex = 37;
            this.hashCachingEnabledBox.Text = "Use hash caching";
            this.hashCachingEnabledBox.UseVisualStyleBackColor = true;
            this.hashCachingEnabledBox.Click += new System.EventHandler(this.hashCachingEnabledBox_Click);
            // 
            // previewCheck
            // 
            this.previewCheck.AutoSize = true;
            this.previewCheck.Location = new System.Drawing.Point(12, 148);
            this.previewCheck.Name = "previewCheck";
            this.previewCheck.Size = new System.Drawing.Size(105, 17);
            this.previewCheck.TabIndex = 40;
            this.previewCheck.Text = "Preview enabled";
            this.previewCheck.UseVisualStyleBackColor = true;
            // 
            // thumbnailSize
            // 
            this.thumbnailSize.Location = new System.Drawing.Point(120, 147);
            this.thumbnailSize.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.thumbnailSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thumbnailSize.Name = "thumbnailSize";
            this.thumbnailSize.Size = new System.Drawing.Size(51, 20);
            this.thumbnailSize.TabIndex = 41;
            this.thumbnailSize.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(177, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(126, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Thumbnail size (max 256)";
            // 
            // deletCacheBtn
            // 
            this.deletCacheBtn.Location = new System.Drawing.Point(133, 95);
            this.deletCacheBtn.Name = "deletCacheBtn";
            this.deletCacheBtn.Size = new System.Drawing.Size(97, 23);
            this.deletCacheBtn.TabIndex = 43;
            this.deletCacheBtn.Text = "Delete Cache";
            this.deletCacheBtn.UseVisualStyleBackColor = true;
            this.deletCacheBtn.Click += new System.EventHandler(this.deletCacheBtn_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(122, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(239, 29);
            this.label6.TabIndex = 44;
            this.label6.Text = "(WARNING: This may have several perfomance impact when sorting by total folder fi" +
    "le count)";
            // 
            // trimCache
            // 
            this.trimCache.Location = new System.Drawing.Point(236, 95);
            this.trimCache.Name = "trimCache";
            this.trimCache.Size = new System.Drawing.Size(75, 23);
            this.trimCache.TabIndex = 45;
            this.trimCache.Text = "Trim Cache";
            this.trimCache.UseVisualStyleBackColor = true;
            this.trimCache.Click += new System.EventHandler(this.trimCache_Click);
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 262);
            this.Controls.Add(this.trimCache);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.deletCacheBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.thumbnailSize);
            this.Controls.Add(this.previewCheck);
            this.Controls.Add(this.hashCachingEnabledBox);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.fileCountEnabledCheckbox);
            this.Controls.Add(this.maxSizeBox);
            this.Controls.Add(this.minSizeBox);
            this.Controls.Add(this.IgnoreEmptyBox);
            this.Controls.Add(this.Cancel_btn);
            this.Controls.Add(this.ok_btn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.usecrcCheck);
            this.Name = "ConfigPanel";
            this.Text = "ConfigPanel";
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox usecrcCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.Button Cancel_btn;
        private System.Windows.Forms.CheckBox IgnoreEmptyBox;
        private System.Windows.Forms.MaskedTextBox minSizeBox;
        private System.Windows.Forms.MaskedTextBox maxSizeBox;
        private System.Windows.Forms.CheckBox fileCountEnabledCheckbox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.CheckBox hashCachingEnabledBox;
        private System.Windows.Forms.CheckBox previewCheck;
        private System.Windows.Forms.NumericUpDown thumbnailSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button deletCacheBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button trimCache;
    }
}