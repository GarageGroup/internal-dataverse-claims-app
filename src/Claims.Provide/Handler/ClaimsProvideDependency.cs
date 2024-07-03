using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Provide.Test")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class ClaimsProvideDependency
{
    public static Dependency<IClaimsProvideHandler> UseClaimsProvideHandler(
        this Dependency<IHttpApi, ClaimsProvideOption> dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IClaimsProvideHandler>(CreateHandler);

        static ClaimsProvideHandler CreateHandler(IHttpApi httpApi, ClaimsProvideOption option)
        {
            ArgumentNullException.ThrowIfNull(httpApi);
            ArgumentNullException.ThrowIfNull(option);

            return new(httpApi, option);
        }
    }
}