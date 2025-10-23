namespace DuplicateFinder.Forms
{
    partial class TrimCacheDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.trimBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(436, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This operation will scan the disk(s) and remove cached data about files that no l" +
    "onger exist.";
            // 
            // trimBtn
            // 
            this.trimBtn.Location = new System.Drawing.Point(12, 74);
            this.trimBtn.Name = "trimBtn";
            this.trimBtn.Size = new System.Drawing.Size(75, 23);
            this.trimBtn.TabIndex = 1;
            this.trimBtn.Text = "TRIM!";
            this.trimBtn.UseVisualStyleBackColor = true;
            this.trimBtn.Click += new System.EventHandler(this.trimBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "This may take VERY LONG TIME";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Enabled = false;
            this.cancelBtn.Location = new System.Drawing.Point(93, 74);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 3;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(284, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Click TRIM to begin cleaning cache. Click Cancel to abort.";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 104);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(437, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // TrimCacheDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 143);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trimBtn);
            this.Controls.Add(this.label1);
            this.Name = "TrimCacheDialog";
            this.Text = "TrimCacheDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button trimBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}