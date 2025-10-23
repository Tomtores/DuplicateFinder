namespace Engine.Entities
{
    public interface IProgressStatus
    {
        WorkState State { get; }
        string Message { get; }
        int FilesFound { get; }
        int Progress { get; }
    }
}