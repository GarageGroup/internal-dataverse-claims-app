using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using GarageGroup.Infra;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CosmosDbUserApi(IHttpApi httpApi, CosmosDbUserApiOption option, IDateProvider dateProvider) : IDbUserApi
{
    private const string TableName = "DataverseUsers";

    private const string SelectedField = "DataverseUserId";

    private const string PartitionKeyFieldName = "PartitionKey";

    private const string RowKeyFieldName = "RowKey";

    private const string ApiVersion = "2019-02-02";

    private const string AcceptHeader = "application/json;odata=nometadata";

    private FlatArray<KeyValuePair<string, string>> BuildHeaders(string date, string stringToSign)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));
        var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

        return 
        [
            new("Authorization", $"SharedKeyLite {option.AccountName}:{Convert.ToBase64String(hash)}"),
            new("x-ms-date", date),
            new("x-ms-version", ApiVersion),
            new("Accept", AcceptHeader)
        ];
    }

    private readonly record struct DataverseUserIdJson
    {
        [JsonPropertyName(SelectedField)]
        public Guid Id { get; init; }
    }

    private readonly record struct DbUserSetJsonOut
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