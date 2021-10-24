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
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
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

                    Dictionary<string, int> messages = null;
                    _memoryCache.TryGetValue<Dictionary<string, int>>("messages", out messages);

                    if (messages == null) messages = new Dictionary<string, int>();

                    Thread.Sleep(100);

                    messages.Remove(content);
                    _memoryCache.Set<Dictionary<string, int>>("messages", messages);

                    //if (messages.Any())
                        Task.Run(async () => await _hub.SendMQMessage(content));

                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                //consumer.Shutdown += OnConsumerShutdown;
                //consumer.Registered += OnConsumerRegistered;
                //consumer.Unregistered += OnConsumerUnregistered;
                //consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

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

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"connection shut down {e.ReplyText}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled {e.ConsumerTags}");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer unregistered {e.ConsumerTags}");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer registered {e.ConsumerTags}");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"consumer shutdown {e.ReplyText}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
