using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Tests.TestDoubles
{
    public class LoggerFactoryTestDouble<T> : ILoggerFactory
    {
        ILogger<T> _logger = null;

        public void AddProvider(ILoggerProvider provider)
        {

        }


        public ILogger CreateLogger(string categoryName)
        {
            if (_logger == null)
                _logger = new LoggerTestDouble<T>();
            return _logger;
        }

        public void Dispose()
        {

        }
    }
}
