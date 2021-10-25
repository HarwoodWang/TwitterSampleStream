using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.SharedLibrary
{
    public class TwitterHistoryStreamProcess
    {
        private static readonly object _fileAccess = new object();
        private ILogger<TwitterHistoryStreamProcess> _logger;

        public TwitterHistoryStreamProcess(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TwitterHistoryStreamProcess>();
        }

        public async Task<List<OutputEntity>> GetTwitterHistorySummary()
        {
            List<OutputEntity> output = new List<OutputEntity>();
            List<StreamDataEntity> lstEntities = new List<StreamDataEntity>();

            try
            {
                string strFileName = "Twitter_Stream_Summary.json"; //config.HistorySummaryFileName;
                string strFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);

                if (!File.Exists(strFullName)) return null;

                lock (_fileAccess)
                {
                    using (StreamReader reader = new StreamReader(strFullName))
                    {
                        while (!reader.EndOfStream)
                        {
                            string strJson = reader.ReadLine();
                            StreamDataEntity entity = JsonConvert.DeserializeObject<StreamDataEntity>(strJson);

                            lstEntities.Add(entity);
                        }
                    }
                }

                output = GetOutput(lstEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("TwitterStreamProcess :: GetTwitterHistorySummary -- {0}", ex.Message));
            }

            return output;
        }

        private List<OutputEntity> GetOutput(List<StreamDataEntity> lstEntities)
        {
            List<OutputEntity> outputs = new List<OutputEntity>();

            try
            {
                IEnumerable<StreamDataEntity> selectedStreams = new List<StreamDataEntity>();

                if (lstEntities.Count > 100)
                {
                    selectedStreams = lstEntities.Take(100);
                }
                else
                {
                    selectedStreams = lstEntities;
                }


                foreach (var entity in selectedStreams)
                {
                    OutputEntity outputentity = new OutputEntity();

                    outputentity.StartTime = entity.StartTime;
                    outputentity.EndTime = entity.EndTime;
                    outputentity.TotalCount = entity.Count;
                    outputentity.TotalMinutes = entity.EndTime.Subtract(entity.StartTime).TotalMinutes;
                    outputentity.AveragePerMinute = entity.Count / outputentity.TotalMinutes;

                    outputs.Add(outputentity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("TwitterStreamProcess :: GetOutput -- {0}", ex.Message));
            }

            return outputs;
        }
    }
}