using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.BackgroundWorker.RabbitMQServices
{
    public interface IPublishService
    {
        bool PubMessages(OutputEntity queues);
    }
}
