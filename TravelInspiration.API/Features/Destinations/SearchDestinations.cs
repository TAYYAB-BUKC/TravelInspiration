using Azure.Data.Tables;
using MediatR;
using TravelInspiration.API.Shared.Domain.Models;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Destinations;

public sealed class SearchDestinations : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/destinations",
             (string? searchFor,
                IMediator mediator,
                CancellationToken cancellationToken) =>
             {
                 return mediator.Send(
                     new SearchDestinationsQuery(searchFor),
                     cancellationToken);
             });
    }

    public sealed class SearchDestinationsQuery(string? searchFor) : IRequest<IResult>
    {
        public string? SearchFor { get; } = searchFor;
    }

    public sealed class DestinationDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        //public required string ImageName { get; set; }
    }

    public sealed class SearchDestinationsHandler(IConfiguration configuration, 
        TableServiceClient tableServiceClient) :
       IRequestHandler<SearchDestinationsQuery, IResult>
    {
        private readonly IConfiguration _configuration = configuration;
		private readonly TableServiceClient _tableServiceClient = tableServiceClient;

		public async Task<IResult> Handle(SearchDestinationsQuery request,
            CancellationToken cancellationToken)
        {
            var destinationsTableClient = _tableServiceClient.GetTableClient("Destinations");

            var filter = request.SearchFor == null ? "" : TableClient.CreateQueryFilter($"Name eq {request.SearchFor}");

			var amountToReturn = _configuration.GetValue<int>("Destinations:AmountToReturn");

            var destinations = new List<DestinationDto>();

			await foreach (var d in destinationsTableClient.QueryAsync<TableEntity>(
	                        filter: filter,
	                        maxPerPage: amountToReturn,
	                        cancellationToken: cancellationToken))
			{
				destinations.Add(new DestinationDto
				{
					Id = d.GetInt32("Id") ?? -1,
					Name = d.GetString("Name")
				});
			}

			// filter destinations based on searchFor 
			//var filteredDestinations = destinations.Where(d =>
			//    request.SearchFor == null ||
			//    d.Name.Contains(request.SearchFor));

			// return results, returning only the amount described in settings
			return Results.Ok(destinations);
        }
    }
}
