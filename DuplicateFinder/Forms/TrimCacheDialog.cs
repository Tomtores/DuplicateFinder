using Engine;
using Engine.Infrastructure;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DuplicateFinder.Forms
{
    public partial class TrimCacheDialog : Form
    {
        private readonly ILogger logger;
        private readonly Guid? installationSalt;
        Thread trimmer;        

        public TrimCacheDialog(Guid? installationSalt, ILogger logger)
        {
            this.installationSalt = installationSalt;
            this.logger = logger;
            InitializeComponent();
        }

        private void trimBtn_Click(object sender, EventArgs e)
        {
            this.trimBtn.Enabled = false;
            this.cancelBtn.Enabled = true;
            this.progressBar1.Value = 0;

            trimmer = new Thread(ExecuteTrim);
            trimmer.Start();
        }

        private void ExecuteTrim()
        {
            try
            {
                FinderFactory.TrimCache(UpdateProgress, installationSalt, logger); // this will hang by design
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateProgress(100);
            EnableTrimButton();
        }

        private void UpdateProgress(int progress)
        {
            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.Invoke(new Action<int>(UpdateProgress), progress);
            }
            else
            {
                this.progressBar1.Value = progress;
            }
        }

        private void EnableTrimButton()
        {
            if (this.trimBtn.InvokeRequired)
            {
                this.trimBtn.Invoke(new Action(EnableTrimButton));
            }
            else
            {
                this.trimBtn.Enabled = true;
                this.cancelBtn.Enabled = false;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            trimmer?.Abort();
            EnableTrimButton();
            this.cancelBtn.Enabled = false;
        }
    }
}
