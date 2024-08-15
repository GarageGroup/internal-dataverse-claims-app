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
            new("Authorization", $"SharedKey {option.AccountName}:{token}"),
            new("x-ms-date", date),
            new("x-ms-version", Version)
        ];
    }

    private string BuildGetSasToken(Guid userId, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        string stringToSign = string.Join("\n",
        [
            "HEAD",     // HTTP метод
            "",         // Content-Encoding
            "",         // Content-Language
            "",         // Content-Length (пустое для GET)
            "",         // Content-MD5
            "",         // Content-Type
            "",         // Date
            "",         // If-Modified-Since
            "",         // If-Match
            "",         // If-None-Match
            "",         // If-Unmodified-Since
            "",         // Range
            $"x-ms-date:{date}\nx-ms-version:{Version}",    // CanonicalizedHeaders
            $"/{option.AccountName}/{option.ContainerName}/{userId}.txt"    // CanonicalizedResource
        ]);

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