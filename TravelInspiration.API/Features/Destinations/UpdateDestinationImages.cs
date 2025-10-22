﻿using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using MediatR;
using System.Text;
using System.Text.Json;
using TravelInspiration.API.Shared.Domain.Models;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Destinations;

public class UpdateDestinationImages : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPut("api/destinations/{destinationId}/images",
            (int destinationId,
            UpdateDestinationImagesCommand updateDestinationImagesCommand,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            {
                // make sure the destinationId from the Uri is used
                updateDestinationImagesCommand.DestinationId = destinationId;
                return mediator.Send(
                  updateDestinationImagesCommand,
                  cancellationToken);
            });
    }

    public sealed class UpdateDestinationImagesCommand : IRequest<IResult>
    {
        public sealed class ImageToUpdate
        {
            public required string Name { get; set; }
            public required string ImageBytes { get; set; }
        }
        public int DestinationId { get; set; }
        public List<ImageToUpdate> ImagesToUpdate { get; set; } = [];
    }


	public sealed class UpdateDestinationImagesCommandHandler(IConfiguration configuration,
		BlobServiceClient blobServiceClient, EventGridPublisherClient eventGridPublisherClient) : //, QueueServiceClient queueServiceClient) :
        IRequestHandler<UpdateDestinationImagesCommand, IResult>
    {
        private readonly IConfiguration _configuration = configuration;
		private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
		private readonly EventGridPublisherClient _eventGridPublisherClient = eventGridPublisherClient;

		//private readonly QueueServiceClient _queueServiceClient = queueServiceClient;

		public async Task<IResult> Handle(UpdateDestinationImagesCommand request,
            CancellationToken cancellationToken)
        {
			var destinationsBlobClient = _blobServiceClient.GetBlobContainerClient("destination-images");

			//var queueClient = _queueServiceClient.GetQueueClient("image-message-queue");

			foreach (var image in request.ImagesToUpdate)
			{
				var blobClient = destinationsBlobClient.GetBlobClient(image.Name);

                if (!blobClient.Exists(cancellationToken).Value)
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(image.ImageBytes)))
                    {
                        await blobClient.UploadAsync(stream, new BlobUploadOptions()
                        {
                            Tags = new Dictionary<string, string>()
                            {
                                { "DestinationIdentifier", Convert.ToString(request.DestinationId) }
                            }
                        });

                        //var message = new
                        //{
                        //    Action = "ImageBlobCreated",
                        //    BlobName = image.Name,
                        //};

                        //await queueClient.SendMessageAsync(JsonSerializer.Serialize(message), cancellationToken);
					}
                }
                else
                {
                    var blobTags = await blobClient.GetTagsAsync(cancellationToken: cancellationToken);

                    if(blobTags.Value.Tags.TryGetValue("DestinationIdentifier", out var destinationId)
                        && destinationId == request.DestinationId.ToString())
                    {
						using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(image.ImageBytes)))
						{
							await blobClient.UploadAsync(stream, new BlobUploadOptions()
							{
								Tags = new Dictionary<string, string>()
							{
								{ "DestinationIdentifier", Convert.ToString(request.DestinationId) }
							}
							});

							//var message = new
							//{
							//	Action = "ImageBlobUpdated",
							//	BlobName = image.Name,
							//};

							//await queueClient.SendMessageAsync(JsonSerializer.Serialize(message), cancellationToken);
						}
					}
                    else
                    {
                        return Results.Problem();
                    }
                }

                var imageCloudEvent = new CloudEvent(
                    $"destinations/{request.DestinationId}/images/{image.Name}",
                    "com.travelinspiration.destination-created-or-updated",
                    new { BlobName = image.Name, DestinationId = request.DestinationId }
                    )
                {
                    Id = Guid.NewGuid().ToString(),
                    Time = DateTimeOffset.Now
                };

                await _eventGridPublisherClient.SendEventAsync(imageCloudEvent);
			}

			return Results.Ok();
        }
    }
}