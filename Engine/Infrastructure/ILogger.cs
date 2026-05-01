namespace Engine.Infrastructure
{
    public interface ILogger
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
    }
}
