using TravelInspiration.API;
using TravelInspiration.API.Shared.Slices;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor(); 
   
builder.Services.RegisterApplicationServices();
builder.Services.RegisterPersistenceServices(builder.Configuration);
builder.Services.AddAzureClients(clientBuilder =>
{
	clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:TravelInspirationTableStorageConnection:blobServiceUri"]!).WithName("ConnectionStrings:TravelInspirationTableStorageConnection");
	clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:TravelInspirationTableStorageConnection:queueServiceUri"]!).WithName("ConnectionStrings:TravelInspirationTableStorageConnection");
	clientBuilder.AddTableServiceClient(builder.Configuration["ConnectionStrings:TravelInspirationTableStorageConnection:tableServiceUri"]!).WithName("ConnectionStrings:TravelInspirationTableStorageConnection");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();
 
app.MapSliceEndpoints();

app.Run();