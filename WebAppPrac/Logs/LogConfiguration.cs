using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Graylog.Extended;

namespace WebAppPrac.Logs
{
    public static class LogConfiguration
    {
        public const string LogDirPropertyName = "LogDir";
        public const string LogNamePropertyName = "LogName";
        public const string LogKeyPropertyName = "LogKey";
        public const string LogTimePropertyName = "LogTime";
        public const string SourceContextPropertyName = "SourceContext";

        private const string LogFolder = "Logs";
        private const string SystemFolder = "System";
        private const string DefaultLogFile = "app..log";
        //private const string DefaultSqliteFile = "svr-log.db3";
        private const string DefaultLogFormat = "Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss.ffff}{NewLine}Message: {Message:lj}{NewLine}Severity: {Level}{NewLine}{Exception}--------------------------------------------------------------------{NewLine}";

        private static LoggerConfiguration LoggerConfiguration { get; set; }

        public static LogOptions Options { get; private set; }

		public static T GetTypedOptions<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class, new()
		{
			services.Configure<T>(configuration.GetSection(sectionName));
			using var serviceProvider = services.BuildServiceProvider();
			return serviceProvider.GetService<IOptions<T>>()?.Value;
		}

		public static void SetupLog(this IServiceCollection services, ref IConfiguration configuration)
        {
            var logOptions = services.GetTypedOptions<LogOptions>(configuration, "Log");

            if (logOptions == null) return;

            using var serviceProvider = services.BuildServiceProvider();

            ConfigureLog(logOptions);

            if (logOptions.UseAOPTrace)
            {
                SetupLogAop(ref services, logOptions);
            }

            if (logOptions.UseGraylog && logOptions.GrayLog != null)
            {
                SetupGraylog(logOptions.GrayLog);
            }

            serviceProvider.GetService<ILoggerFactory>().AddSerilog();
        }

        private static void SetupLogAop(ref IServiceCollection services, LogOptions logOptions)
        {
            services.AddTransient<AopControllerAttribute>();
            services.AddTransient<AopExceptionFilterAttribute>();

            services.AddMvc(options =>
            {
                options.Filters.Add(new AopControllerAttribute(logOptions.UseAOPTrace));
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AopExceptionFilterAttribute());
            });
        }

        public static void ConfigureLog(LogOptions logOptions)
        {
            Options = logOptions;

            LoggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console();  //NOSONAR false positive

            if (logOptions.UseLogFile)
            {
                var defaultLogPath = Path.Combine(LogFolder, SystemFolder, DefaultLogFile);
                LoggerConfiguration = LoggerConfiguration.WriteTo.File(GetValidPath(logOptions.LogFile, defaultLogPath), rollingInterval: RollingInterval.Day, outputTemplate: DefaultLogFormat);
            }

            if (!string.IsNullOrEmpty(logOptions.SeqLogServerHost))
            {
                LoggerConfiguration = LoggerConfiguration.WriteTo.Seq(logOptions.SeqLogServerHost);
            }

            // DB에 로그남기는 기능은 보류 (2022.04.28)
            //if (logOptions.UseSQLiteDb)
            //{
            //    LoggerConfiguration = LoggerConfiguration.WriteTo.SQLite(GetValidPath(logOptions.SqliteDatabase, DefaultSqliteFile), retentionPeriod: TimeSpan.FromMinutes(2), retentionCheckInterval: TimeSpan.FromSeconds(10));
            //}

            SetLogLevel(logOptions.LogLevel, LoggerConfiguration);

            LoggerConfiguration = LoggerConfiguration
                            .Enrich.FromLogContext()
                            .WriteTo.Logger(lc => lc
                                .Enrich.With<LogFileNameEnricher>()
                                .WriteTo.Map(LogFileNameEnricher.LogFilePathPropertyName, (logFilePath, wt) =>
                                wt.Map(LogFileNameEnricher.LogFileTemplatePropertyName, (logFileTemplate, wt2) =>
                                wt2.Map(LogFileNameEnricher.RollingIntervalPropertyName, (rollingInterval, wt3) =>
                                wt3.File(GetValidPath(null, logFilePath),
                                    outputTemplate: logFileTemplate,
                                    rollingInterval: (RollingInterval)Enum.Parse(typeof(RollingInterval), rollingInterval))))));

            Log.Logger = LoggerConfiguration.CreateLogger();
        }

        private static void SetupGraylog(GraylogOptions graylogOptions)
        {
            if (graylogOptions == null) return;
            var graylogConfig = new GraylogSinkConfiguration
            {
                GraylogTransportType = GetGraylogTransportTypeFromString(graylogOptions.GrayLogProtocol),
                Host = graylogOptions.GrayLogHost,
                Port = graylogOptions.GrayLogPort,
                UseSecureConnection = graylogOptions.UseSecureConnection,
                UseAsyncLogging = graylogOptions.UseAsyncLogging,
                RetryCount = graylogOptions.RetryCount,
                RetryIntervalMs = graylogOptions.RetryIntervalMs,
                MaxUdpMessageSize = graylogOptions.MaxUdpMessageSize
            };

            LoggerConfiguration = LoggerConfiguration.WriteTo.Graylog(graylogConfig);
        }

        private static GraylogTransportType GetGraylogTransportTypeFromString(string transportType)
        {
            return transportType.ToLower() switch
            {
                "tcp" => GraylogTransportType.Tcp,
                "udp" => GraylogTransportType.Udp,
                "http" => GraylogTransportType.Http,
                _ => GraylogTransportType.Udp
            };
        }

        private static string GetValidPath(string inputPath, string optionalFileName)
        {
            if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(Path.GetFileName(inputPath)))
            {
                return @"C:\Users\PC\source\repos\WebAppPrac\";
            }

            return Path.GetFullPath(inputPath);
        }

        private static void SetLogLevel(string logEventLevel, LoggerConfiguration loggerConfiguration)
        {
            if (string.IsNullOrEmpty(logEventLevel)) return;
            switch (logEventLevel.ToLower())
            {
                case "warning":
                    loggerConfiguration.MinimumLevel.Warning();
                    return;
                case "verbose":
                    loggerConfiguration.MinimumLevel.Verbose();
                    return;
                case "fatal":
                    loggerConfiguration.MinimumLevel.Fatal();
                    return;
                case "error":
                    loggerConfiguration.MinimumLevel.Error();
                    return;
                case "information":
                    loggerConfiguration.MinimumLevel.Information();
                    return;
                default:
                    loggerConfiguration.MinimumLevel.Debug();
                    return;
            }
        }
    }
}
