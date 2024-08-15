using GarageGroup.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<Unit, Failure<Unit>>> CreateUserAsync(
        DbUserCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            BuildHttpSendCreateIn)
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            Unit.From,
            MapHttpFailure);

    private HttpSendIn BuildHttpSendCreateIn(DbUserCreateIn input)
    {
        var date = dateProvider.Date.ToString("R");

        return new(
            method: HttpVerb.Post,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
        {
            Headers = BuildHeaders(date, $"{date}\n/{option.AccountName}/{TableName}"),
            Body = HttpBody.SerializeAsJson(new DbUserCreateJson()
            {
                DataverseUserId = input.DataverseUserId,
                PartitionKey = input.AzureUserId,
                RowKey = input.AzureUserId,
            }),
            SuccessType = HttpSuccessType.OnlyStatusCode
        };
    }
}