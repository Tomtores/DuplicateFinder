namespace Engine.Infrastructure
{
    internal class NullLogger : ILogger
    {
        public void Error(string message)
        {
            // do nothing
        }

        public void Info(string message)
        {
            // do nothing
        }

        public void Warning(string message)
        {
            // do nothing
        }
    }
}
