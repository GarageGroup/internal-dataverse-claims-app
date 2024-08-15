using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class BlobStorageUserApiDependency
{
    public static Dependency<IDbUserApi> UseBlobStorageUserApi(
        this Dependency<IHttpApi, BlobStorageUserApiOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IDbUserApi>(CreateApi);

        static BlobStorageUserApi CreateApi(IHttpApi httpApi, BlobStorageUserApiOption option)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(httpApi, option, DateProvider.Instance);
        }
    }
}