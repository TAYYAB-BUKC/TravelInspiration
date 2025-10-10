using Azure.Storage.Queues;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text.Json;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Destinations;

public sealed class ProcessDestinationImageChanges : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/processdestinationimagechanges",
             (IMediator mediator,
                CancellationToken cancellationToken) =>
             {
                 return mediator.Send(
                     new ProcessDestinationImageChangesQuery(),
                     cancellationToken);
             }); 
    }

    public sealed class ProcessDestinationImageChangesQuery() : IRequest<IResult>
    {
    }

    public sealed class MessageDto
    {
        public required string Action { get; set; }
        public required string BlobName { get; set; } 
    }

    public sealed class ProcessDestinationImageChangesHandler(QueueServiceClient queueServiceClient) :
       IRequestHandler<ProcessDestinationImageChangesQuery, IResult>
    {
		private readonly QueueServiceClient _queueServiceClient = queueServiceClient;

		public async Task<IResult> Handle(ProcessDestinationImageChangesQuery request,
            CancellationToken cancellationToken)
        {
            var queueClient = _queueServiceClient.GetQueueClient("image-message-queue");

            var messages = await queueClient.ReceiveMessagesAsync(maxMessages: 2, cancellationToken: cancellationToken);

            var messageList = new List<MessageDto>();
            
            foreach (var message in messages.Value)
            {
                var messageText = JsonSerializer.Deserialize<MessageDto>(message.Body);
                if (!string.IsNullOrEmpty(messageText.BlobName))
                {
                    messageList.Add(messageText);
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                }
            }

            return Results.Ok(messageList);
        }
    } 
} 