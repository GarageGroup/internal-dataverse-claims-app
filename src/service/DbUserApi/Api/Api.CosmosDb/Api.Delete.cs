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
            Unit.From,
            MapHttpFailure);

    private HttpSendIn BuildHttpSendDeleteIn(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}(PartitionKey='{userId}',RowKey='{userId}')";

        return new(
            method: HttpVerb.Delete,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}(PartitionKey='{userId}',RowKey='{userId}')")
        {
            Headers = BuildHeaders(date, stringToSign),
            SuccessType = HttpSuccessType.OnlyStatusCode
        };
    }
}