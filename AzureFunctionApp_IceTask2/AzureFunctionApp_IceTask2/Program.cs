using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client.Extensions.Msal;

var host = new HostBuilder()
    
    .ConfigureServices(services =>
    {
       services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton(provider =>
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureTableStorage")
                ?? "UseDevelopmentStorage=true";

            var tableServiceClient = new TableServiceClient(connectionString);
            var tableClient = tableServiceClient.GetTableClient("Users");
            tableClient.CreateIfNotExists();

            return tableClient;
        });
    })
    .Build();

host.Run();


// References
// Azurite(local Azure Storage emulator) —
  //https://learn.microsoft.com/azure/storage/common/storage-use-azurite
//Microsoft Learn, *Azure Functions HTTP trigger* —
 // https://learn.microsoft.com/azure/azure-functions/functions-bindings-http-webhook-trigger
//Microsoft Learn, *Azure Table Storage client library for .NET (Azure.Data.Tables)* —
 // https://learn.microsoft.com/dotnet/api/overview/azure/data.tables-readme
//Microsoft Learn, *Guide for running C# Azure Functions in an isolated worker process* —
 // https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide
