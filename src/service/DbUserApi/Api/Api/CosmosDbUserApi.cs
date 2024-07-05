using System;
using System.Text.Json.Serialization;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CosmosDbUserApi(IHttpApi httpApi, CosmosDbUserApiOption option, IDateProvider dateProvider) : IDbUserApi
{
    private const string TableName = "DataverseUsers";

    private const string SelectedField = "DataverseUserId";

    private const string ApiVersion = "2019-02-02";

    private const string AcceptHeader = "application/json;odata=nometadata";

    private readonly record struct DataverseUserIdJson
    {
        [JsonPropertyName(SelectedField)]
        public Guid Id { get; init; }
    }
}