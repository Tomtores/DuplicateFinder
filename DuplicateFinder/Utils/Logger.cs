using DuplicateFinder.Enums;
using Engine.Infrastructure;
using System;
using System.Diagnostics;

namespace DuplicateFinder.Utils
{
    internal class Logger : ILogger, IDisposable
    {
        private readonly TraceSource trace;
        private readonly TextWriterTraceListener fileListener;

        public Logger(LogLevel loggingLevel)
        {
            if (loggingLevel == LogLevel.None)
            {
                trace = new TraceSource("NoLogging", SourceLevels.Off);
                return;
            }

            fileListener = new TextWriterTraceListener($"log.txt");
            fileListener.Filter = new EventTypeFilter(GetLoggingLevel(loggingLevel));

            trace = new TraceSource(DateTime.Today.ToString("yyyy-MM-dd"), SourceLevels.All);
            trace.Listeners.Clear();
            trace.Listeners.Add(fileListener);

            Trace.AutoFlush = true;
        }

        private SourceLevels GetLoggingLevel(LogLevel loggingLevel)
        {
            switch(loggingLevel)
            {
                case LogLevel.Error:
                    return SourceLevels.Error;
                case LogLevel.Warning:
                    return SourceLevels.Warning;
                case LogLevel.All:
                    return SourceLevels.All;
                default:
                    return SourceLevels.Warning;
            }
        }

        public void Error(string message)
        {
            trace.TraceEvent(TraceEventType.Error, 0, message);
            trace.Flush();
        }

        public void Warning(string message)
        {
            trace.TraceEvent(TraceEventType.Warning, 0, message);
        }

        public void Info(string message)
        {
            trace.TraceEvent(TraceEventType.Information, 0, message);
        }

        public void Flush()
        {
            trace.Flush();
            fileListener?.Flush();
        }

        public void Dispose()
        {
            this.Flush();
            trace.Close();
            fileListener?.Close();
        }
    }
}
