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
            Body = new()
            {
                Content = new(FileContent),
                Type = new(MediaTypeNames.Text.Plain)
            },
            SuccessType = HttpSuccessType.OnlyStatusCode
        };
    

    private FlatArray<KeyValuePair<string, string>> BuildCreateHeaders(DbUserCreateIn input)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildCreateSasToken(input, date);

        return
        [
            new(AuthorizationHeaderName, $"SharedKey {option.AccountName}:{token}"),
            new(DateHeaderName, date),
            new(VersionHeaderName, Version),
            new($"{MetadataHeaderName}-{MetadataDataverseUserIdName}", input.DataverseUserId.ToString()),
            new($"{MetadataHeaderName}-{MetadataAzureUserIdName}", input.AzureUserId.ToString()),
            new("x-ms-blob-type", "BlockBlob")
        ];
    }

    private string BuildCreateSasToken(DbUserCreateIn input, string date)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        var canonicalizedHeaders = $"x-ms-blob-type:BlockBlob\n" +
            $"{DateHeaderName}:{date}\n" +
            $"{MetadataHeaderName}-{MetadataAzureUserIdName}:{input.AzureUserId}\n" +
            $"{MetadataHeaderName}-{MetadataDataverseUserIdName}:{input.DataverseUserId}\n" +
            $"{VersionHeaderName}:{Version}";

        var canonicalizedResource = $"/{option.AccountName}/{option.ContainerName}/{input.AzureUserId}.txt";

        var stringToSign = BuildStringToSign(
            "PUT", FileContent.Length.ToString(), MediaTypeNames.Text.Plain, canonicalizedHeaders, canonicalizedResource);

        var signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

        return Convert.ToBase64String(signatureBytes);
    }
}