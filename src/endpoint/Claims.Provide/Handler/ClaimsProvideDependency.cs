using System;
using System.Runtime.CompilerServices;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Endpoint.Claims.Provide.Test")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class ClaimsProvideDependency
{
    public static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler<TDbUserApi>(this Dependency<TDbUserApi> dependency)
        where TDbUserApi : IDbUserGetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<IClaimsProvideHandler>(CreateHandler);

        static ClaimsProvideHandler CreateHandler(TDbUserApi dbUserApi)
        {
            ArgumentNullException.ThrowIfNull(dbUserApi);
            return new(dbUserApi);
        }
    }
}