using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStreamProduce.SharedLibrary.Models
{
    public class StreamDataEntity
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long Count { get; set; }
    }
}
