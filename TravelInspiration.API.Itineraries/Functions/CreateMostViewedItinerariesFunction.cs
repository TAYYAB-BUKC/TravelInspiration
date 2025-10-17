using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;
using TravelInspiration.API.Itineraries.Models;

namespace TravelInspiration.API.Itineraries.Functions
{
	public class CreateMostViewedItinerariesFunction
    {
        private readonly ILogger<CreateMostViewedItinerariesFunction> _logger;

        public CreateMostViewedItinerariesFunction(ILogger<CreateMostViewedItinerariesFunction> logger)
        {
            _logger = logger;
        }

        [Function("CreateMostViewedItinerariesFunction")]
		[OpenApiOperation("CreateMostViewedItineraries", "CreateMostViewedItineraries", Description = "Create a list of most-viewed itineraries.")]
        [OpenApiRequestBody("application/json", typeof(List<ItineraryDto>), Description = "Create a list of most-viewed itineraries.")]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ItineraryDto))]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "mostvieweditineraries")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			if (string.IsNullOrWhiteSpace(requestBody))
			{
				throw new Exception("Request body is missing");
			}

            var itineraries = JsonSerializer.Deserialize<List<ItineraryDto>>(requestBody, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            
            return new OkObjectResult(itineraries?.FirstOrDefault());
        }
    }
}