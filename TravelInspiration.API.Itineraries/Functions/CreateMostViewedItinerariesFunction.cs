using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "mostvieweditineraries")] HttpRequest req)
        {
            if(req.Body.Length <= 0)
            {
                throw new Exception("Request body is missing");
            }
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var itineraries = JsonSerializer.Deserialize<List<ItineraryDto>>(requestBody);
            
            return new OkObjectResult(new
            {
                StatusCode = HttpStatusCode.OK,
                Items = itineraries?.FirstOrDefault()
            });
        }
    }
}