using System.Net;
using Azure.Data.Tables;
using AzureFunctionUserApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionUserApp.Functions
{
   
    public class GetAllUsersFunction
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<GetAllUsersFunction> _logger;

        public GetAllUsersFunction(TableClient tableClient, ILogger<GetAllUsersFunction> logger)
        {
            _tableClient = tableClient;
            _logger = logger;
        }

        [Function("GetAllUsers")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestData req)
        {
            var users = new List<UserDto>();

            await foreach (var entity in _tableClient.QueryAsync<UserEntity>(e => e.PartitionKey == "User"))
            {
                users.Add(new UserDto
                {
                    UserId = entity.RowKey,
                    Name = entity.Name,
                    Surname = entity.Surname,
                    Email = entity.Email,
                    ContactNumber = entity.ContactNumber
                });
            }

            _logger.LogInformation("Returning {Count} users.", users.Count);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(users);
            return response;
        }
    }
}