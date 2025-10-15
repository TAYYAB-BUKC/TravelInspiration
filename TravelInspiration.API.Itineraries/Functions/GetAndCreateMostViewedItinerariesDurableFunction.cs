using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using TravelInspiration.API.Itineraries.Models;

namespace TravelInspiration.API.Itineraries.Functions
{
	public static class GetAndCreateMostViewedItinerariesDurableFunction
    {
        [Function(nameof(GetAndCreateMostViewedItinerariesDurableFunction))]
        public static async Task<List<ItineraryDto>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var hostAddress = context.GetInput<string>();

            var httpRetryOptions = new HttpRetryOptions()
            {
                MaxNumberOfAttempts = 5,
                FirstRetryInterval = TimeSpan.FromSeconds(5),
            };

			var getItinerariesResponse = await context.CallHttpAsync(
                HttpMethod.Get,
                new Uri($"{hostAddress}/itineraries"),
                retryOptions: httpRetryOptions);

            if(getItinerariesResponse.StatusCode != HttpStatusCode.OK)
            {
                return new List<ItineraryDto>();
            }

			var createMostViewedItinerariesResponse = await context.CallHttpAsync(
				HttpMethod.Post,
				new Uri($"{hostAddress}/mostvieweditineraries"),
				getItinerariesResponse.Content,
				retryOptions: httpRetryOptions);

			if (createMostViewedItinerariesResponse.StatusCode != HttpStatusCode.OK)
			{
				return new List<ItineraryDto>();
			}

			var mostviewedItineraries = JsonSerializer.Deserialize<List<ItineraryDto>>(createMostViewedItinerariesResponse.Content, new JsonSerializerOptions(JsonSerializerDefaults.Web));

			return mostviewedItineraries;
        }

        [Function("GetAndCreateMostViewedItinerariesDurableFunction_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
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
