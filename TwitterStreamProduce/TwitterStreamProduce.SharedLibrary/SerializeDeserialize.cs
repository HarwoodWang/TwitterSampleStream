using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.SharedLibrary
{
    public class SerializeDeserialize
    {
        public static string SerializeObjects(ConcurrentQueue<StreamDataEntity> queues)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            string strQueueJson = System.Text.Json.JsonSerializer.Serialize<List<StreamDataEntity>>(queues.ToList(), options);

            return strQueueJson;
        }

        public static string SerializeObject(StreamDataEntity entity)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            string strQueueJson = System.Text.Json.JsonSerializer.Serialize<StreamDataEntity>(entity, options);

            return strQueueJson;
        }
    }
}
