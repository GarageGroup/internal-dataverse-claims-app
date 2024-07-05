using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class CosmosDbUserApiDependency
{
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