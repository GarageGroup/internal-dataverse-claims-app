using GarageGroup.Infra;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GarageGroup.Internal.Dataverse.Claims;

partial class BlobStorageUserApi
{
    public async ValueTask<Result<DbUserSetGetOut, Failure<Unit>>> GetUsersAsync(
        Unit input, CancellationToken cancellationToken)
    {
        var result = new List<DbUserSetGetItem>();
        string? nextMarker = null;

        while (true) 
        {
            var response = await GetUsersXmlAsync(nextMarker, cancellationToken);
            if (response.IsFailure)
            {
                return response.FailureOrThrow();
            }

            var xmlData = (response.SuccessOrThrow().Body.Content?.ToString()).OrEmpty();
            byte[] byteArray = Encoding.UTF8.GetBytes(xmlData);
            string cleanedXml = Encoding.UTF8.GetString(byteArray, GetBOMLength(byteArray), byteArray.Length - GetBOMLength(byteArray));
            var doc = XDocument.Parse(cleanedXml);

            var blobs = doc.Descendants("Blob");
            foreach (var blob in blobs)
            {
                var metadata = blob.Element("Metadata");
                if (metadata != null)
                {
                    string? dataverseUserId = null;
                    string? azureUserId = null;
                    foreach (var metadataItem in metadata.Elements())
                    {
                        var tag = metadataItem.Name.LocalName;
                        if (tag == "dataverseuserid")
                        {
                            dataverseUserId = metadataItem.Value;
                        }

                        if (tag == "azureuserid")
                        {
                            azureUserId = metadataItem.Value;
                        }

                        if (string.IsNullOrWhiteSpace(dataverseUserId) is false && string.IsNullOrWhiteSpace(azureUserId) is false)
                        {
                            result.Add(
                                new(
                                    azureUserId: Guid.Parse(azureUserId),
                                    dataverseUserId: Guid.Parse(dataverseUserId)));
                            break;
                        }
                    }
                }
            }

            nextMarker = doc.Root?.Element("NextMarker")?.Value;
            if (string.IsNullOrEmpty(nextMarker))
            {
                break;
            }
        }

        return new DbUserSetGetOut()
        {
            Users = result,
        };

        static int GetBOMLength(byte[] byteArray)
        {
            if (byteArray.Length >= 3 &&
                byteArray[0] == 0xEF &&
                byteArray[1] == 0xBB &&
                byteArray[2] == 0xBF)
            {
                return 3;
            }
            return 0;
        }
    }

    private ValueTask<Result<HttpSendOut, Failure<Unit>>> GetUsersXmlAsync(
        string? input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            BuildHttpSendGetSetIn)
        .PipeValue(
            httpApi.SendAsync)
        .MapFailure(
            MapHttpFailure);

    private HttpSendIn BuildHttpSendGetSetIn(string? nextMarker)
    {
        var uri = $"https://{option.AccountName}.blob.core.windows.net/{option.ContainerName}?restype=container&comp=list&include=metadata";
        if (string.IsNullOrEmpty(nextMarker) is false)
        {
            uri += $"&marker={nextMarker}";
        }

        return new(
            method: HttpVerb.Get,
            requestUri: uri)
        {
            Headers = BuildGetSetHeaders(nextMarker)
        };
    }
        

    private FlatArray<KeyValuePair<string, string>> BuildGetSetHeaders(string? nextMarker)
    {
        var date = dateProvider.Date.ToString("R");
        var token = BuildGetSetSasToken(date, nextMarker);

        return
        [
            new("Authorization", $"SharedKey {option.AccountName}:{token}"),
            new("x-ms-date", date),
            new("x-ms-version", Version)
        ];
    }

    private string BuildGetSetSasToken(string date, string? nextMarker)
    {
        using var hashAlgorithm = new HMACSHA256(Convert.FromBase64String(option.AccountKey));

        var canonicalizedResource = $"/{option.AccountName}/{option.ContainerName}\ncomp:list\ninclude:metadata\nrestype:container";
        if (string.IsNullOrEmpty(nextMarker) is false)
        {
            canonicalizedResource = $"/{option.AccountName}/{option.ContainerName}\ncomp:list\ninclude:metadata\nmarker:{nextMarker}\nrestype:container";
        }

        string stringToSign = string.Join("\n",
        [
            "GET",  // HTTP метод
            "",     // Content-Encoding
            "",     // Content-Language
            "",     // Content-Length (пустое для GET)
            "",     // Content-MD5
            "",     // Content-Type
            "",     // Date
            "",     // If-Modified-Since
            "",     // If-Match
            "",     // If-None-Match
            "",     // If-Unmodified-Since
            "",     // Range
            $"x-ms-date:{date}\nx-ms-version:{Version}",    // CanonicalizedHeaders
            canonicalizedResource   // CanonicalizedResource
        ]);

        byte[] signatureBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return Convert.ToBase64String(signatureBytes);
    }
}