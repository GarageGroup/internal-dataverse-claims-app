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
            MapHttpFailure);

    private HttpSendIn BuildHttpSendGetSetIn()
    {
        var date = dateProvider.Date.ToString("R");

        return new(
            method: HttpVerb.Get,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
        {
            Headers = BuildHeaders(date, $"{date}\n/{option.AccountName}/{TableName}")
        };
    }

    private static DbUserSetGetItem MapDbUserSetGetItem(DbUserJson user)
        =>
        new(
            azureUserId: user.AzureUserId,
            dataverseUserId: user.DataverseUserId);
}