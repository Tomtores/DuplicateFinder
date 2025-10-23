namespace Engine.Entities
{
    /// <summary>
    /// Stores progress status for search.
    /// </summary>
    internal class ProgressStatus : IProgressStatus
    {
        public WorkState State { get; set; }
        public string Message { get; set; }
        public int FilesFound { get; set; }
        public int Progress { get; set; }
        
        public ProgressStatus()
        {
            this.State = WorkState.Iddle;
        }
    }
}