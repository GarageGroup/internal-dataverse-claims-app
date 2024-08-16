using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class DbUserApiDependency
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

    public static Dependency<IDbUserApi> UseCosmosDbUserApi(
        this Dependency<IHttpApi, CosmosDbUserApiOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IDbUserApi>(CreateApi);

        static CosmosDbUserApi CreateApi(IHttpApi httpApi, CosmosDbUserApiOption option)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(httpApi, option, DateProvider.Instance);
        }
    }
}