using System.Net;
using Azure;
using Azure.Data.Tables;
using AzureFunctionUserApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionUserApp.Functions
{
   
    public class GetUserFunction
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<GetUserFunction> _logger;

        public GetUserFunction(TableClient tableClient, ILogger<GetUserFunction> logger)
        {
            _tableClient = tableClient;
            _logger = logger;
        }

        [Function("GetUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}")] HttpRequestData req,
            string userId)
        {
            try
            {
                var result = await _tableClient.GetEntityAsync<UserEntity>(
                    partitionKey: "User",
                    rowKey: userId);

                var dto = new UserDto
                {
                    UserId = result.Value.RowKey,
                    Name = result.Value.Name,
                    Surname = result.Value.Surname,
                    Email = result.Value.Email,
                    ContactNumber = result.Value.ContactNumber
                };

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(dto);
                return response;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogInformation("User {UserId} was not found.", userId);
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteAsJsonAsync(new { error = $"User '{userId}' was not found." });
                return notFound;
            }
        }
    }
}