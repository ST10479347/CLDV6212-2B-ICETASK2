using Azure;
using Azure.Data.Tables;

namespace AzureFunctionUserApp.Models
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "User";
        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;

        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
    }
}