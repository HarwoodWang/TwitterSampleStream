using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamProduce.BackgroundWorker.RabbitMQServices;
using TwitterStreamProduce.BackgroundWorker.TwitterStreamServices;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.BackgroundWorker.BackgroundTasks
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ILogger<ScopedProcessingService> _logger;

        private readonly TwitterStreamDataConifiguration _config;
        private readonly SecretKeyConfiguration _configSecretKey;
        private readonly ILoggerFactory _loggery;
        private readonly IPublishService _mqServices;

        public ScopedProcessingService(TwitterStreamDataConifiguration config,
                                    SecretKeyConfiguration configSecretKey,
                                    ILoggerFactory loggery,
                                    IPublishService mqServices)
        {
            _loggery = loggery;
            _logger = loggery.CreateLogger<ScopedProcessingService>(); ;
            _config = config;
            _configSecretKey = configSecretKey;
            _mqServices = mqServices;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(string.Format("ScopedProcessingService :: DoWork starts"));

            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    TwitterStreamService _twitterStreamService = new TwitterStreamService(_config, _configSecretKey, _loggery, _mqServices);
                    await _twitterStreamService.RunTwitterStreamReader(stoppingToken);
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
