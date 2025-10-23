using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DuplicateFinder
{
    public partial class MultifolderSelect : Form
    {
        public IOrderedEnumerable<string> Result { get; private set; }

        public MultifolderSelect(Func<string> lastDirGet, Action<string> lastDirSet, IEnumerable<string> lastValue = null)
        {
            InitializeComponent();
            this.folderList1.Configure(lastDirGet, lastDirSet, lastValue);
        }               

        private void Save_btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Result = this.folderList1.Items;
            this.Close();
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }        
    }
}
