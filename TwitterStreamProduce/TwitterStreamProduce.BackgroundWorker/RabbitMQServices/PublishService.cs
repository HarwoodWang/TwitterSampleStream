using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TwitterStreamProduce.SharedLibrary;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.BackgroundWorker.RabbitMQServices
{
    public class PublishService : IPublishService
    {
        private readonly ILogger<PublishService> _logger;
        private int _messageCount = 1;
        private readonly IMemoryCache _memoryCache;

        public PublishService(ILoggerFactory loggery, IMemoryCache memoryCache)
        {
            this._logger = loggery.CreateLogger<PublishService>();
            this._memoryCache = memoryCache;
        }

        public bool PubMessages(OutputEntity queue)
        {
            _logger.LogInformation("Start : Getting stream summary in method PublishService::PubMessages()");

            string strQueueJson = SerializeDeserialize.SerializeObject(queue);
            bool bPubMesage = true;

            try
            {
                //Main entry point to the RabbitMQ .NET AMQP client
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "randomStreamData",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                         var body = Encoding.UTF8.GetBytes(strQueueJson);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "randomStreamData",
                                             basicProperties: null,
                                             body: body);
                    }
                }

                bPubMesage = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("PublishService::PubMessages() -- {0}", ex.Message));
                bPubMesage = false;
            }

            _logger.LogInformation("Completed in method PublishService::PubMessages()");

            return bPubMesage;
        }
    }
}
