using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamProduce.SharedLibrary
{
    public class LoggerConfig
    {
        public static ILoggerFactory GetLoggerConfig<T>() where T : class
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            ILoggerFactory factory = serviceProvider.GetService<ILoggerFactory>();
            return factory;
        }
    }
}
