using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPrac.Logs
{
    public class LogOptions
    {
        public bool UseLogFile { get; set; }
        public bool UseSQLiteDb { get; set; }
        public bool UseGraylog { get; set; }
        public bool UseAOPTrace { get; set; }
        public string LogLevel { get; set; }
        public string SqliteDatabase { get; set; }
        public string LogFile { get; set; }
        public string SeqLogServerHost { get; set; }
        public GraylogOptions GrayLog { get; set; }
        public List<LogFileTarget> FileTargets { get; set; }
        public List<LogDBTarget> DBTargets { get; set; }
    }
}
