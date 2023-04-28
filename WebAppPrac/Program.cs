using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.Net;
using WebAppPrac;
using Serilog;

Host.CreateDefaultBuilder(args)
	.UseWindowsService() // uessystemd Ãß°¡µÊ
	.ConfigureServices(services =>
	{
	})
	.ConfigureWebHostDefaults(webBuilder => {
		webBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) => {
			var env = hostingContext.HostingEnvironment;
			var os = Environment.OSVersion;
			configBuilder
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"appsettings.{env.EnvironmentName}.{os.Platform}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

			//SetupKestrel.Configure(webBuilder, configBuilder.Build());
		})
		.UseContentRoot(Directory.GetCurrentDirectory())
		.UseStartup<Startup>(); })
		.UseSerilog()
		.UseSystemd()
	.Build()
	.Run();
