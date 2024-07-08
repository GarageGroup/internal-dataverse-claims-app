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
            _ => default(Unit),
            static failure => failure.ToStandardFailure().WithFailureCode(default(Unit)));

    private HttpSendIn BuildHttpSendCreateIn(DbUserCreateIn input)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}";

        var signature = BuildSignature(stringToSign);

        return new(
            method: HttpVerb.Post,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
        {
            Headers =
            [
                new("Authorization", $"SharedKeyLite {option.AccountName}:{signature}"),
                new("x-ms-date", date),
                new("x-ms-version", ApiVersion),
                new("Accept", AcceptHeader)
            ],
            Body = HttpBody.SerializeAsJson(new DbUserCreateJson()
            {
                DataverseUserId = input.DataverseUserId,
                PartitionKey = input.AzureUserId,
                RowKey = input.AzureUserId,
            })
        };
    }
}