using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelInspiration.API.Itineraries.Persistence;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices((builder, services) =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();

		services.AddDbContext<TravelInspirationDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("TravelInspirationDbConnection"), options =>
			{
				options.EnableRetryOnFailure();
			}));
	})
	.Build();

host.Run();