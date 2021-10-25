using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamProduce.BackgroundWorker.RabbitMQServices;
using TwitterStreamProduce.SharedLibrary.Models;
using TwitterStreamProduce.SharedLibrary;

namespace TwitterStreamProduce.BackgroundWorker.TwitterStreamServices
{
    public class TwitterStreamService : ITwitterStreamService
    {
        private string _Url;
        private string _BearerToken;

        private static readonly object obj = new object();
        private readonly ILoggerFactory _loggery;
        private readonly ILogger<TwitterStreamService> _logger;

        private string strHistorySummaryFileName;
        private double StreamIntervalTimeSpan;
        private int maxQueueCount;
        private IPublishService _mqServices;

        public SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);


        private ConcurrentQueue<OutputEntity> collection;

        public TwitterStreamService(TwitterStreamDataConifiguration config,
                                SecretKeyConfiguration configSecretKey,
                                ILoggerFactory loggery,
                                IPublishService mqServices)
        {
            _Url = configSecretKey.Url;
            _BearerToken = configSecretKey.BearerToken;

            this._loggery = loggery;
            this._logger = loggery.CreateLogger<TwitterStreamService>();

            this.strHistorySummaryFileName = config.HistorySummaryFileName;
            this.StreamIntervalTimeSpan = config.StreamTimeSpan;
            this.maxQueueCount = config.MaxQueueCount;

            _mqServices = mqServices;
            collection = new ConcurrentQueue<OutputEntity>();
        }

        public async Task RunTwitterStreamReader(CancellationToken ct)
        {
            _logger.LogDebug("Start : Getting stream summary in method StreamService::GetRandomStream()");

            try
            {
                string strToken = string.Format("Bearer {0}", _BearerToken);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_Url);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Timeout = -1;
                request.Headers["Authorization"] = strToken;

                long totalCount = 0;
                DateTime dtStart = DateTime.Now;

                await _semaphoregate.WaitAsync();

                using (HttpWebResponse response = (HttpWebResponse)await Extensions.GetResponseAsync(request, ct))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                do
                                {
                                    double timeSpan = DateTime.Now.Subtract(dtStart).TotalMinutes;
                                    Interlocked.Increment(ref totalCount);

                                    if (timeSpan >= 0.10)
                                    {
                                        var task = Task.Run(() =>
                                        {
                                            int minuteTotalCount = Convert.ToInt32(Math.Floor(totalCount / 1000.0));

                                            long totalcount = Convert.ToInt64(Math.Floor(totalCount / 1000.0));
                                            double totalminutes = Math.Round(DateTime.Now.Subtract(dtStart).TotalMinutes, 3);

                                            OutputEntity minuteEntity = new OutputEntity()
                                            {
                                                StartTime = dtStart,
                                                EndTime = DateTime.Now,
                                                TotalCount = totalcount,
                                                TotalMinutes = totalminutes,
                                                AveragePerMinute = totalcount / totalminutes
                                            };

                                            dtStart = DateTime.Now;
                                            totalCount = totalCount - minuteTotalCount;

                                            collection.Enqueue(minuteEntity);
                                            return new Tuple<ConcurrentQueue<OutputEntity>, OutputEntity>(collection, minuteEntity);
                                        });

                                        await task.ContinueWith((tpl) => OutputToFile.WriteSummaryToFile(tpl.Result.Item2, strHistorySummaryFileName));
                                        await task.ContinueWith((tpl) => _mqServices.PubMessages(tpl.Result.Item2));
                                    }
                                } while (!reader.EndOfStream);
                            }
                        }
                    }
                }

                _semaphoregate.Release();

                if (ct.IsCancellationRequested)
                {
                    long totalcount = Convert.ToInt64(Math.Floor(totalCount / 1000.0));
                    double totalminutes = Math.Round(DateTime.Now.Subtract(dtStart).TotalMinutes, 3);

                    OutputEntity minuteEntity = new OutputEntity()
                    {
                        StartTime = dtStart,
                        EndTime = DateTime.Now,
                        TotalCount = totalcount,
                        TotalMinutes = totalminutes,
                        AveragePerMinute = totalcount / totalminutes
                    };

                    collection.Enqueue(minuteEntity);

                    await Task.Run(() => OutputToFile.WriteSummaryToFile(minuteEntity, strHistorySummaryFileName));
                    await Task.Run(() => _mqServices.PubMessages(minuteEntity));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("StreamService::GetRandomStream() -- {0}", ex.ToString()));
            }

            _logger.LogDebug("Completed in method StreamService::GetRandomStream()");
        }
    }
}
