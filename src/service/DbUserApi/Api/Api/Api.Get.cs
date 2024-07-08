using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<DbUserGetOut, Failure<DbUserGetFailureCode>>> GetUserAsync(
        DbUserGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input.AzureUserId, cancellationToken)
        .Pipe(
            BuildHttpSendGetIn)
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            static success => new DbUserGetOut(
                dataverseUserId: success.Body.DeserializeFromJson<DataverseUserIdJson>().Id),
            static failure => failure.ToStandardFailure().MapFailureCode(MapFailureCode));

    private HttpSendIn BuildHttpSendGetIn(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}(PartitionKey='{userId}',RowKey='{userId}')";

        var signature = BuildSignature(stringToSign);

        return new(
            method: HttpVerb.Get,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/" +
                $"{TableName}(PartitionKey='{userId}',RowKey='{userId}')?$select={SelectedField}")
        {
            Headers =
            [
                new("Authorization", $"SharedKeyLite {option.AccountName}:{signature}"),
                new("x-ms-date", date),
                new("x-ms-version", ApiVersion),
                new("Accept", AcceptHeader)
            ]
        };
    }

    private static DbUserGetFailureCode MapFailureCode(HttpFailureCode failureCode)
        =>
        failureCode switch
        {
            HttpFailureCode.NotFound => DbUserGetFailureCode.NotFound,
            _ => DbUserGetFailureCode.Unknown
        };
}