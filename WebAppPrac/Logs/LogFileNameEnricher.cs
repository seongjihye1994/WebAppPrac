using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WebAppPrac.Logs
{
    public class LogFileNameEnricher : ILogEventEnricher
    {
        private Dictionary<string, List<LogEventProperty>> cache = new Dictionary<string, List<LogEventProperty>>();

        internal const string LogFilePathPropertyName = "LogFilePath";
        internal const string LogFileTemplatePropertyName = "LogFileTemplate";
        internal const string DefaultTemplate = "{Message:lj}{NewLine}";
        internal const string RollingIntervalPropertyName = "RollingInterval";
        internal const string DefaultRollingInterval = "Infinite";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var sourceContext = logEvent.Properties.GetValueOrDefault(LogConfiguration.SourceContextPropertyName, null);
            if (sourceContext == null || string.IsNullOrEmpty(sourceContext.ToString()))
                return;

            var loggerName = sourceContext.ToString().Trim('\"');
            if (!cache.TryGetValue(loggerName, out var props))
            {
                var fileName = $"{loggerName}-.log";
                var template = DefaultTemplate;
                var rollingInterval = DefaultRollingInterval;
                var target = GetFileTarget(loggerName);
                if (target != null)
                {
                    if (!string.IsNullOrEmpty(target.FileName))
                        fileName = target.FileName;

                    if (!string.IsNullOrEmpty(target.Template))
                        template = target.Template;

                    if (!string.IsNullOrEmpty(target.RollingInterval) && Enum.TryParse(target.RollingInterval, out RollingInterval interval))
                        rollingInterval = interval.ToString();
                }

                var fileNameProp = propertyFactory.CreateProperty(LogFilePathPropertyName, fileName);
                var templateProp = propertyFactory.CreateProperty(LogFileTemplatePropertyName, template);
                var rollingIntervalProp = propertyFactory.CreateProperty(RollingIntervalPropertyName, rollingInterval);

                props = new List<LogEventProperty>();
                props.Add(fileNameProp);
                props.Add(templateProp);
                props.Add(rollingIntervalProp);

                cache.Add(loggerName, props);
            }

            foreach (var prop in props)
                logEvent.AddPropertyIfAbsent(prop);
        }

        private LogFileTarget GetFileTarget(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName))
                return null;

            var option = LogConfiguration.Options;
            if (option == null)
                return null;

            var targets = option.FileTargets;
            if (targets == null)
                return null;

            foreach (var target in targets)
            {
                if (target.IsMatched(loggerName))
                    return target;
            }

            return targets.Where(x => x.LoggerName == "Default").FirstOrDefault();
        }
    }
}
