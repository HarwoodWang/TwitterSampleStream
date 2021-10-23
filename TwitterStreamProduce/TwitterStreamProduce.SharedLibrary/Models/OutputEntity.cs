using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamProduce.SharedLibrary.Models
{
    public class OutputEntity
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long TotalCount { get; set; }

        public double AveragePerMinutes { get; set; }

        public double TotalMinutes { get; set; }
    }
}
