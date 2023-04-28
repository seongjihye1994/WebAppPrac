using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Net.Http.Headers;
using Serilog;
using System.Reflection;
using WebAppPrac.Controllers;
using WebAppPrac.Logs;

namespace WebAppPrac
{
    public class Startup
	{
		IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IConfiguration Configuration => _configuration;


		// 로거


		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.SetupLog(ref _configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
			IWebHostEnvironment env,
			IHostApplicationLifetime lifetime)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Configure the HTTP request pipeline.
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}



			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				// health checks
				//endpoints.MapHealthChecks("/health");
				//endpoints.MapGrpcService<HealthServiceImpl>();
				endpoints.MapControllers();

			});
		}
	}
}

