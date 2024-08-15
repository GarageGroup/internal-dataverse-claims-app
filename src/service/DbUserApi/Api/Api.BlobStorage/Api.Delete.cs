using GarageGroup.Infra;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class BlobStorageUserApi
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
        =>
        new(
            method: HttpVerb.Delete,
            requestUri: $"https://{option.AccountName}.blob.core.windows.net/{option.ContainerName}/{userId}.txt")
        {
            Headers = BuildDeleteHeaders(userId),
        };

    private FlatArray<KeyValuePair<string, string>> BuildDeleteHeaders(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildDeleteSasToken(userId, date);

        return
        [
            new("Authorization", $"SharedKey {option.AccountName}:{token}"),
            new("x-ms-date", date),
            new("x-ms-version", Version),
            new("x-ms-delete-snapshots", "include")
        ];
    }

    private string BuildDeleteSasToken(Guid userId, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        string stringToSign = string.Join("\n",
        [
            "DELETE",   // HTTP метод
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
            $"x-ms-date:{date}\nx-ms-delete-snapshots:include\nx-ms-version:{Version}", // CanonicalizedHeaders
            $"/{option.AccountName}/{option.ContainerName}/{userId}.txt"    // CanonicalizedResource
        ]);

        byte[] signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return Convert.ToBase64String(signatureBytes);
    }
}