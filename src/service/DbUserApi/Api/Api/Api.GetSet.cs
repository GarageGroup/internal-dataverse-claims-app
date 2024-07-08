using GarageGroup.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class CosmosDbUserApi
{
    public ValueTask<Result<DbUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            _ => BuildHttpSendGetSetIn())
        .PipeValue(
            httpApi.SendAsync)
        .Map(
            static success => new DbUserSetGetOut()
            {
                Users = success.Body.DeserializeFromJson<DbUserSetJson>().Value.Map(MapDbUserSetGetItem)
            },
            static failure => failure.ToStandardFailure().WithFailureCode(default(Unit)));

    private HttpSendIn BuildHttpSendGetSetIn()
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}";

        var signature = BuildSignature(stringToSign);

        return new(
            method: HttpVerb.Get,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
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

    private static DbUserSetGetItem MapDbUserSetGetItem(DbUserJson user)
        =>
        new(
            azureUserId: user.AzureUserId,
            dataverseUserId: user.DataverseUserId);
}