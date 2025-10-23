namespace DuplicateFinder
{
    partial class MultifolderSelect
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
            this.Save_btn = new System.Windows.Forms.Button();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.folderList1 = new FolderList();
            this.SuspendLayout();
            // 
            // Save_btn
            // 
            this.Save_btn.Location = new System.Drawing.Point(346, 270);
            this.Save_btn.Name = "Save_btn";
            this.Save_btn.Size = new System.Drawing.Size(75, 23);
            this.Save_btn.TabIndex = 0;
            this.Save_btn.Text = "Save";
            this.Save_btn.UseVisualStyleBackColor = true;
            this.Save_btn.Click += new System.EventHandler(this.Save_btn_Click);
            // 
            // cancel_btn
            // 
            this.cancel_btn.Location = new System.Drawing.Point(427, 270);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(75, 23);
            this.cancel_btn.TabIndex = 1;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = true;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // folderList1
            // 
            this.folderList1.Location = new System.Drawing.Point(13, 13);
            this.folderList1.Name = "folderList1";
            this.folderList1.Size = new System.Drawing.Size(489, 251);
            this.folderList1.TabIndex = 2;
            // 
            // MultifolderSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 308);
            this.Controls.Add(this.folderList1);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.Save_btn);
            this.Name = "MultifolderSelect";
            this.Text = "MultifolderSelect";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Save_btn;
        private System.Windows.Forms.Button cancel_btn;
        private FolderList folderList1;
    }
}