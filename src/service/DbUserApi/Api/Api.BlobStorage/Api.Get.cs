using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class BlobStorageUserApi
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
                dataverseUserId: Guid.Parse(success.Headers.AsEnumerable().FirstOrDefault(a => a.Key == "x-ms-meta-dataverseuserid").Value)),
            static failure => failure.ToStandardFailure().MapFailureCode(MapFailureCode));

    private HttpSendIn BuildHttpSendGetIn(Guid userId)
        =>
        new(
            method: HttpVerb.Head,
            requestUri: $"https://{option.AccountName}.blob.core.windows.net/{option.ContainerName}/{userId}.txt")
        {
            Headers = BuildGetHeaders(userId),
        };

    private FlatArray<KeyValuePair<string, string>> BuildGetHeaders(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildGetSasToken(userId, date);

        return
        [
            new(AuthorizationHeaderName, $"SharedKey {option.AccountName}:{token}"),
            new(DateHeaderName, date),
            new(VersionHeaderName, Version)
        ];
    }

    private string BuildGetSasToken(Guid userId, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        var canonicalizedHeaders = $"{DateHeaderName}:{date}\n{VersionHeaderName}:{Version}";
        var canonicalizedResource = $"/{option.AccountName}/{option.ContainerName}/{userId}.txt";

        string stringToSign = BuildStringToSign("HEAD", null, null, canonicalizedHeaders, canonicalizedResource);
        byte[] signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

        return Convert.ToBase64String(signatureBytes);
    }

    private static DbUserGetFailureCode MapFailureCode(HttpFailureCode failureCode)
        =>
        failureCode switch
        {
            HttpFailureCode.NotFound => DbUserGetFailureCode.NotFound,
            _ => DbUserGetFailureCode.Unknown
        };
}