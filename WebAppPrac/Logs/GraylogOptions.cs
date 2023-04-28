using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPrac.Logs
{
    public class GraylogOptions
    {
        public string GrayLogHost { get; set; }
        public int GrayLogPort { get; set; }
        public string GrayLogProtocol { get; set; }
        public bool UseSecureConnection { get; set; }
        public bool UseAsyncLogging { get; set; }
        public int RetryCount { get; set; }
        public int RetryIntervalMs { get; set; }
        public int MaxUdpMessageSize { get; set; }
    }
}
