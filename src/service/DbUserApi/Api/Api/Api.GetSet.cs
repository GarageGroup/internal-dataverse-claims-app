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
                Users = success.Body.DeserializeFromJson<DbUserSetJsonOut>().Value.Map(MapDbUserSetGetItem)
            },
            static failure => failure.ToStandardFailure().WithFailureCode(default(Unit)));

    private HttpSendIn BuildHttpSendGetSetIn()
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}";

        return new(
            method: HttpVerb.Get,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
        {
            Headers = BuildHeaders(date, stringToSign)
        };
    }

    private static DbUserSetGetItem MapDbUserSetGetItem(DbUserJson user)
        =>
        new(
            azureUserId: user.AzureUserId,
            dataverseUserId: user.DataverseUserId);
}