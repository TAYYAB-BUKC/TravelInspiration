using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System.Data;
using TravelInspiration.API.Itineraries.Models;

namespace TravelInspiration.API.Itineraries
{
	public class GetItinerariesFunction
	{
        private readonly ILogger<GetItinerariesFunction> _logger;

        public GetItinerariesFunction(ILogger<GetItinerariesFunction> logger)
        {
            _logger = logger;
        }

        [Function("GetItinerariesFunction")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "itineraries")] HttpRequest req,
			[SqlInput(commandText: "SELECT Id, Name, Description, UserId FROM Itineraries WITH (NOLOCK)" +
								   //"\nWHERE NAME LIKE @searchFor" // Not Working
								   "\nWHERE NAME = @searchFor", // Working
					  commandType: CommandType.Text, parameters: "@searchFor={Query.searchFor}", connectionStringSetting: "TravelInspirationDbConnection")] IEnumerable<ItineraryDto> itineraries)
        {
            _logger.LogInformation("C# HTTP trigger function processed a itineraries request.");
            return new OkObjectResult(itineraries);
        }
    }
}
