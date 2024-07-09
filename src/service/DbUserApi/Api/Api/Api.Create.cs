﻿using GarageGroup.Infra;
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
            static failure => failure.ToStandardFailure().WithFailureCode(default(Unit)));

    private HttpSendIn BuildHttpSendCreateIn(DbUserCreateIn input)
    {
        var date = dateProvider.Date.ToString("R");
        var stringToSign = $"{date}\n/{option.AccountName}/{TableName}";

        return new(
            method: HttpVerb.Post,
            requestUri: $"https://{option.AccountName}.table.cosmos.azure.com/{TableName}")
        {
            Headers = BuildHeaders(date, stringToSign),
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