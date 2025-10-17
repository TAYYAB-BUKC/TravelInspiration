using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading;
using TravelInspiration.API.Itineraries.Models;
using TravelInspiration.API.Itineraries.Persistence;

namespace TravelInspiration.API.Itineraries.Functions
{
	public class GetItinerariesFunction
	{
		private readonly ILogger<GetItinerariesFunction> _logger;
		private readonly TravelInspirationDbContext _dbContext;
		private readonly IConfiguration _configuration;

		public GetItinerariesFunction(ILogger<GetItinerariesFunction> logger, TravelInspirationDbContext dbContext, IConfiguration configuration)
		{
			_logger = logger;
			_dbContext = dbContext;
			_configuration = configuration;
		}

		[Function("GetItinerariesFunction")]
		[OpenApiOperation("GetItineraries", "GetItineraries", Description = "Get the Itineraries.")]
		[OpenApiParameter("SearchFor", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Search for the Itineraries by part of their name or description.")]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<ItineraryDto>))]
		public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "itineraries")] HttpRequest req)
		{
			_logger.LogInformation("C# HTTP trigger function processed a itineraries request.");

			var searchForValue = Convert.ToString(req.Query["searchFor"]);

			if (!int.TryParse(_configuration["MaxAmountOfItinerariesToReturn"], out int maxAmountOfItinerariesToReturn))
			{
				throw new Exception("MaxAmountOfItinerariesToReturn setting is missing");
			}

			var itineraries = await _dbContext.Itineraries
									 .Where(i =>
									 searchForValue == null ||
									 i.Name.Contains(searchForValue) ||
									 i.Description != null && i.Description.Contains(searchForValue))
									 .AsNoTracking()
									 .Take(maxAmountOfItinerariesToReturn)
									 .ToListAsync();

			var itinerariesDto = itineraries.Select(i => new ItineraryDto
			{
				Id = i.Id,
				Name = i.Name,
				Description = i.Description,
				UserId = i.UserId
			});

			return new OkObjectResult(itinerariesDto);
		}
	}
}
