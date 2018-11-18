using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Tests.TestDoubles
{
    public class LoggerTestDouble<T> : ILogger<T>, IDisposable
    {
        public static List<int> ErrorEvents = new List<int>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            ErrorEvents.Add(eventId.Id);
        }
    }
}
