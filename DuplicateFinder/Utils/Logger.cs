using Engine.Infrastructure;
using System;
using System.Diagnostics;

namespace DuplicateFinder.Utils
{
    internal class Logger : ILogger
    {
        private TraceSource trace;
        private TextWriterTraceListener fileListener;

        public Logger(SourceLevels loggingLevel)
        {
            fileListener = new TextWriterTraceListener($"log.txt");
            fileListener.Filter = new EventTypeFilter(loggingLevel);

            trace = new TraceSource(DateTime.Today.ToString("yyyy-MM-dd"), SourceLevels.All);
            trace.Listeners.Clear();
            trace.Listeners.Add(fileListener);

            Trace.AutoFlush = true;
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
            fileListener.Flush();
        }
    }
}
