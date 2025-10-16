using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TravelInspiration.API.Itineraries.Models;

namespace TravelInspiration.API.Itineraries.Functions
{
	public class GetAndCreateMostViewedItinerariesDurableFunction(IConfiguration configuration)
    {
		private readonly IConfiguration _configuration = configuration;

		[Function(nameof(GetAndCreateMostViewedItinerariesDurableFunction))]
        public async Task<ItineraryDto> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var hostAddress = context.GetInput<string>();
            var getItinerariesFunctionKey = _configuration["GetItinerariesFunctionKey"];
			var createMostViewedItinerariesFunctionKey = _configuration["CreateMostViewedItinerariesFunctionKey"];

			var httpRetryOptions = new HttpRetryOptions()
            {
                MaxNumberOfAttempts = 5,
                FirstRetryInterval = TimeSpan.FromSeconds(5),
            };

			var getItinerariesResponse = await context.CallHttpAsync(
                HttpMethod.Get,
                new Uri($"{hostAddress}/itineraries?code{getItinerariesFunctionKey}"),
                retryOptions: httpRetryOptions);

            if(getItinerariesResponse.StatusCode != HttpStatusCode.OK)
            {
                return null!;
            }

			var createMostViewedItinerariesResponse = await context.CallHttpAsync(
				HttpMethod.Post,
				new Uri($"{hostAddress}/mostvieweditineraries?code{createMostViewedItinerariesFunctionKey}"),
				getItinerariesResponse.Content,
				retryOptions: httpRetryOptions);

			if (createMostViewedItinerariesResponse.StatusCode != HttpStatusCode.OK)
			{
				return null!;
			}

			var mostviewedItinerary = JsonSerializer.Deserialize<ItineraryDto>(createMostViewedItinerariesResponse.Content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

			return mostviewedItinerary;
        }

        [Function("GetAndCreateMostViewedItinerariesDurableFunction_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "generatemostvieweditineraries")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("GetAndCreateMostViewedItinerariesDurableFunction_HttpStart");

            var hostAddress = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}" +
                              $"{req.Url.LocalPath.Substring(0, req.Url.LocalPath.IndexOf("generatemostvieweditineraries") - 1)}";

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(GetAndCreateMostViewedItinerariesDurableFunction), hostAddress);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
