using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using AzureFunctionUserApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionUserApp.Functions
{
    public class CreateUserFunction
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<CreateUserFunction> _logger;

        public CreateUserFunction(TableClient tableClient, ILogger<CreateUserFunction> logger)
        {
            _tableClient = tableClient;
            _logger = logger;
        }

        [Function("CreateUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequestData req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            UserDto? dto;
            try
            {
                dto = JsonSerializer.Deserialize<UserDto>(
                    requestBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse request body as JSON.");
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Request body must be valid JSON." });
                return badRequest;
            }

            if (dto is null || string.IsNullOrWhiteSpace(dto.UserId))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "UserId is required." });
                return badRequest;
            }

            var entity = new UserEntity
            {
                RowKey = dto.UserId,
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                ContactNumber = dto.ContactNumber
            };

            // Upsert so re-running the same request in Postman doesn't fail with a 409 Conflict.
            await _tableClient.UpsertEntityAsync(entity);

            _logger.LogInformation("Stored user {UserId} in Table Storage.", dto.UserId);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(dto);
            return response;
        }
    }
}