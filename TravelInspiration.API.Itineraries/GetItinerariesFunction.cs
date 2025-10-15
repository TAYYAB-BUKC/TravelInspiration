using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using TravelInspiration.API.Itineraries.Models;
using TravelInspiration.API.Itineraries.Persistence;

namespace TravelInspiration.API.Itineraries
{
	public class GetItinerariesFunction
	{
        private readonly ILogger<GetItinerariesFunction> _logger;
		private readonly TravelInspirationDbContext _dbContext;

		public GetItinerariesFunction(ILogger<GetItinerariesFunction> logger, TravelInspirationDbContext dbContext)
        {
            _logger = logger;
			_dbContext = dbContext;
		}

        [Function("GetItinerariesFunction")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "itineraries")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a itineraries request.");

            var searchForValue = Convert.ToString(req.Query["searchFor"]);

			var itineraries = await _dbContext.Itineraries
							         .Where(i =>
							         searchForValue == null ||
							         i.Name.Contains(searchForValue) ||
							         (i.Description != null && i.Description.Contains(searchForValue)))
							         .AsNoTracking()
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
