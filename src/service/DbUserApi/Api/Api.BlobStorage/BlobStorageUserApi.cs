using System;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class BlobStorageUserApi(IHttpApi httpApi, BlobStorageUserApiOption option, IDateProvider dateProvider) : IDbUserApi
{
    private const string Version = "2022-11-02";

    private static Failure<Unit> MapHttpFailure(HttpSendFailure failure)
        =>
        failure.ToStandardFailure().WithFailureCode<Unit>(default);
}