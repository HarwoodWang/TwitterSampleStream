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
    public class ConsumeService : IConsumeService
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
        }

        public void RunConsumeMQ(CancellationToken stoppingToken)
        {
            var obs = Observable.Interval(TimeSpan.FromSeconds(10), Scheduler.ThreadPool);
            obs.Subscribe(async x =>
            {
                await ExecuteRabbitMQ(stoppingToken);
            });
        }

        private Task ExecuteRabbitMQ(CancellationToken stoppingToken)
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
                consumer.Received += (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    string strConsumed = System.Text.Encoding.UTF8.GetString(body);

                    Dictionary<string, int> messages = null;
                    _memoryCache.TryGetValue<Dictionary<string, int>>("messages", out messages);

                    if (messages == null) messages = new Dictionary<string, int>();

                    Thread.Sleep(100);

                    messages.Remove(strConsumed);
                    _memoryCache.Set<Dictionary<string, int>>("messages", messages);

                    Task.Run(async () => await _hub.SendMQMessage(messages.OrderBy(m => m.Value).Select(m => m.Key).ToString()));

                    _channel.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume(queue: "randomStreamData",
                                        autoAck: true,
                                        consumer: consumer);


            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("ConsumeService::ExecuteRabbitMQ() -- {0}", ex.ToString()));
            }

            return Task.CompletedTask;
        }
    }
}
