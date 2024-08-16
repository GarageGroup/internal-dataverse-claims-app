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
            SuccessType = HttpSuccessType.OnlyStatusCode
        };

    private FlatArray<KeyValuePair<string, string>> BuildDeleteHeaders(Guid userId)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildDeleteSasToken(userId, date);

        return
        [
            new(AuthorizationHeaderName, $"SharedKey {option.AccountName}:{token}"),
            new(DateHeaderName, date),
            new(VersionHeaderName, Version),
            new("x-ms-delete-snapshots", "include")
        ];
    }

    private string BuildDeleteSasToken(Guid userId, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        var canonicalizedHeaders = $"{DateHeaderName}:{date}\nx-ms-delete-snapshots:include\n{VersionHeaderName}:{Version}";
        var canonicalizedResource = $"/{option.AccountName}/{option.ContainerName}/{userId}.txt";

        var stringToSign = BuildStringToSign("DELETE", null, null, canonicalizedHeaders, canonicalizedResource);
        var signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

        return Convert.ToBase64String(signatureBytes);
    }
}