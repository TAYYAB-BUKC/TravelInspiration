using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
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

		var defaultAzureCredentials = new DefaultAzureCredential();

		var accessTokenResponse = defaultAzureCredentials.GetToken(new TokenRequestContext(["https://database.windows.net/.default"]));

		var connection = new SqlConnection(builder.Configuration.GetConnectionString("TravelInspirationDbConnection"))
		{
			AccessToken = accessTokenResponse.Token
		};

		services.AddDbContext<TravelInspirationDbContext>(options =>
			options.UseSqlServer(connection, options =>
			{
				options.EnableRetryOnFailure();
			}));
	})
	.Build();

host.Run();