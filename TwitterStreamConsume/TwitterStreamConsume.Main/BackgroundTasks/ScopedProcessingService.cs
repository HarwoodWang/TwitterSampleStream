using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamConsume.Main.Hubs;
using TwitterStreamConsume.Main.RabbitMQServices;

namespace TwitterStreamConsume.Main.BackgroundTasks
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger<ScopedProcessingService> _logger;
        private readonly ILoggerFactory _loggery;
        private readonly IConsumeService _mqServices;

        public ScopedProcessingService(ILoggerFactory loggery, IConsumeService mqServices)
        {
            _loggery = loggery;
            _logger = loggery.CreateLogger<ScopedProcessingService>();

            _mqServices = mqServices;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(string.Format("ScopedProcessingService :: DoWork starts"));

            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await _mqServices.ExecuteRabbitMQ(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error : ScopedProcessingService :: DoWork -- {0}", ex.ToString()));
            }

            _logger.LogInformation(string.Format("ScopedProcessingService :: DoWork ends"));
        }
    }
}
