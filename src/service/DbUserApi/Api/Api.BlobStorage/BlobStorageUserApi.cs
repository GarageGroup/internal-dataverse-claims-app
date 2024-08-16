using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class BlobStorageUserApi(IHttpApi httpApi, BlobStorageUserApiOption option, IDateProvider dateProvider) : IDbUserApi
{
    private const string FileContent = "1";

    private const string AuthorizationHeaderName = "Authorization";

    private const string DateHeaderName = "x-ms-date";

    private const string VersionHeaderName = "x-ms-version";

    private const string Version = "2022-11-02";

    private const string MetadataHeaderName = "x-ms-meta";

    private const string MetadataDataverseUserIdName = "dataverseuserid";

    private const string MetadataAzureUserIdName = "azureuserid";

    private static Failure<Unit> MapHttpFailure(HttpSendFailure failure)
        =>
        failure.ToStandardFailure().WithFailureCode<Unit>(default);

    private static string BuildStringToSign(
        string verb, string? contentLength, string? contentType, string canonicalizedHeaders, string canonicalizedResource)
        =>
        string.Join(
            separator: "\n",
            value: [
                verb,                           // HTTP метод
                string.Empty,                   // Content-Encoding
                string.Empty,                   // Content-Language
                contentLength.OrEmpty(),  // Content-Length
                string.Empty,                   // Content-MD5
                contentType.OrEmpty(),    // Content-Type
                string.Empty,                   // Date
                string.Empty,                   // If-Modified-Since
                string.Empty,                   // If-Match
                string.Empty,                   // If-None-Match
                string.Empty,                   // If-Unmodified-Since
                string.Empty,                   // Range
                canonicalizedHeaders,           // CanonicalizedHeaders
                canonicalizedResource           // CanonicalizedResource
            ]);
}