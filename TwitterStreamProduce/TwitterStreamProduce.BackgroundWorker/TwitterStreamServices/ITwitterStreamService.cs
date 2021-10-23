using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStreamProduce.BackgroundWorker.TwitterStreamServices
{
    public interface ITwitterStreamService
    {
        Task RunTwitterStreamReader(CancellationToken ct);
    }
}
