using GarageGroup.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> DeleteUserAsync(
        DbUserDeleteIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input.AzureUserId, cancellationToken)
        .Pipe(
            BuildHttpSendDeleteIn)
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            _ => default(Unit),
            static failure => failure.ToStandardFailure().WithFailureCode(default(Unit)));

    private HttpSendIn BuildHttpSendDeleteIn(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}(PartitionKey='{userId}',RowKey='{userId}')";

        var signature = BuildSignature(stringToSign);

        return new(
            method: HttpVerb.Delete,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/" +
                $"{TableName}(PartitionKey='{userId}',RowKey='{userId}')")
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
}