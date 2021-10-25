using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStreamConsume.Main.RabbitMQServices
{
    public interface IConsumeService
    {
        void RunConsumeMQ(CancellationToken stoppingToken);
    }
}
