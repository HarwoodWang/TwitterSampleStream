using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamConsume.Main.Hubs;

namespace TwitterStreamConsume.Main
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger<ConsumeRabbitMQHostedService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private IMemoryCache _memoryCache;
        private MessageHub _hub;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IMemoryCache memoryCache, MessageHub hub)
        {
            this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();

            _memoryCache = memoryCache;
            _hub = hub;

            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory 
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            // create connection
            _connection = factory.CreateConnection();

            // create channel
            _channel = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                _channel.QueueDeclare(queue: "randomStreamData",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (ch, ea) =>
                {
                    // received message
                    byte[] body = ea.Body.ToArray();
                    string content = System.Text.Encoding.UTF8.GetString(body);

                    Task.Run(async () => await _hub.SendMQMessage(content));

                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume(queue: "randomStreamData",
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format("ConsumeRabbitMQHostedService::ExecuteAsync() -- {0}", ex.ToString()));
            }

            return Task.CompletedTask;
        }   

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
