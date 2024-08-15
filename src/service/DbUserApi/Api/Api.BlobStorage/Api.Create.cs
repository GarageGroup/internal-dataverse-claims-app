using GarageGroup.Infra;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class BlobStorageUserApi
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
        =>
        new(
            method: HttpVerb.Put,
            requestUri: $"https://{option.AccountName}.blob.core.windows.net/{option.ContainerName}/{input.AzureUserId}.txt")
        {
            Headers = BuildCreateHeaders(input),
            Body = new HttpBody()
            {
                Content = new("1"),
                Type = new(MediaTypeNames.Text.Plain)
            },
            SuccessType = HttpSuccessType.OnlyStatusCode,
        };
    

    private FlatArray<KeyValuePair<string, string>> BuildCreateHeaders(DbUserCreateIn input)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildCreateSasToken(input, date);

        return
        [
            new("Authorization", $"SharedKey {option.AccountName}:{token}"),
            new("x-ms-date", date),
            new("x-ms-version", Version),
            new("x-ms-meta-dataverseuserid", input.DataverseUserId.ToString()),
            new("x-ms-meta-azureuserid", input.AzureUserId.ToString()),
            new("x-ms-blob-type", "BlockBlob")
        ];
    }

    private string BuildCreateSasToken(DbUserCreateIn input, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        string stringToSign = string.Join("\n",
        [
            "PUT",          // HTTP метод
            "",             // Content-Encoding
            "",             // Content-Language
            "1",            // Content-Length (пустое для GET)
            "",             // Content-MD5
            "text/plain",   // Content-Type
            "",             // Date
            "",             // If-Modified-Since
            "",             // If-Match
            "",             // If-None-Match
            "",             // If-Unmodified-Since
            "",             // Range
            $"x-ms-blob-type:BlockBlob\nx-ms-date:{date}\nx-ms-meta-azureuserid:{input.AzureUserId}\nx-ms-meta-dataverseuserid:{input.DataverseUserId}\nx-ms-version:{Version}",    // CanonicalizedHeaders
            $"/{option.AccountName}/{option.ContainerName}/{input.AzureUserId}.txt" // CanonicalizedResource
        ]);

        byte[] signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return Convert.ToBase64String(signatureBytes);
    }
}