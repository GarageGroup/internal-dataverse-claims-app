using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CosmosDbUserApi(IHttpApi httpApi, CosmosDbUserApiOption option, IDateProvider dateProvider) : IDbUserApi
{
    private const string TableName = "DataverseUsers";

    private const string SelectedField = "DataverseUserId";

    private const string PartitionKeyFieldName = "PartitionKey";

    private const string RowKeyFieldName = "RowKey";

    private const string ApiVersion = "2019-02-02";

    private const string AcceptHeader = "application/json;odata=nometadata";

    private string BuildSignature(string stringToSign)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));
        var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        
        return Convert.ToBase64String(hash);
    }

    private readonly record struct DataverseUserIdJson
    {
        [JsonPropertyName(SelectedField)]
        public Guid Id { get; init; }
    }

    private readonly record struct DbUserSetJson
    {
        [JsonPropertyName("value")]
        public FlatArray<DbUserJson> Value { get; init; }
    }

    private sealed record class DbUserJson
    {
        [JsonPropertyName(SelectedField)]
        public Guid DataverseUserId { get; init; }

        [JsonPropertyName(RowKeyFieldName)]
        public Guid AzureUserId { get; init; }
    }

    private sealed record class DbUserCreateJson
    {
        [JsonPropertyName(SelectedField)]
        public Guid DataverseUserId { get; init; }

        [JsonPropertyName(RowKeyFieldName)]
        public Guid RowKey { get; init; }

        [JsonPropertyName(PartitionKeyFieldName)]
        public Guid PartitionKey { get; init; }
    }
}