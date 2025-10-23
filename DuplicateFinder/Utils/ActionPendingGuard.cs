using System.Windows.Forms;

namespace DuplicateFinder
{
    /// <summary>
    /// a quickly hacked protection to prevent user from messing with ui while actions are perfoemed. also shows currently ongoing action?
    /// </summary>
    public class ActionPendingGuard
    {
        private readonly Control[] controls;

        public bool IsWorking { get; private set; }

        public ActionPendingGuard(params Control[] controls)
        {
            this.controls = controls;
        }

        public void NotifyAction(bool isWorking)
        {
            foreach(var control in controls)
            {
                control.Enabled = !isWorking;
                control.Refresh();
            }
        }
    }
}