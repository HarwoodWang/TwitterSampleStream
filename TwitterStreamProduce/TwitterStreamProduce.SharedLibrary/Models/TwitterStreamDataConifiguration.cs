using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamProduce.SharedLibrary.Models
{
    public class TwitterStreamDataConifiguration
    {
        public int MaxQueueCount { get; set; }

        public double StreamTimeSpan { get; set; }

        public string HistorySummaryFileName { get; set; }
    }
}
