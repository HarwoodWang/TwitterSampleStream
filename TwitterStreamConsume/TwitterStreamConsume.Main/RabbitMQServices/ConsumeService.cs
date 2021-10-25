using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamConsume.Main.Hubs;

namespace TwitterStreamConsume.Main.RabbitMQServices
{
    public class ConsumeService : IConsumeService, IDisposable
    {
        private readonly ILogger<ConsumeService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private IMemoryCache _memoryCache;
        private MessageHub _hub;

        public ConsumeService(ILoggerFactory loggerFactory, IMemoryCache memoryCache, MessageHub hub)
        {
            this._logger = loggerFactory.CreateLogger<ConsumeService>();

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

        public Task ExecuteRabbitMQ(CancellationToken stoppingToken)
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

                // received message
                consumer.Received += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    string strConsumed = System.Text.Encoding.UTF8.GetString(body);
                    await _hub.SendMQMessage(strConsumed);

                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

                _channel.BasicConsume(queue: "randomStreamData",
                                        autoAck: false,
                                        consumer: consumer);


            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("ConsumeService::ExecuteRabbitMQ() -- {0}", ex.ToString()));
            }

            return Task.CompletedTask;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
