using System;
using System.Runtime.CompilerServices;
using GarageGroup.Infra;
using PrimeFuncPack;

[assembly: InternalsVisibleTo("GarageGroup.Internal.Dataverse.Claims.Service.CrmUserApi.Test")]

namespace GarageGroup.Internal.Dataverse.Claims;

public static class CrmUserApiDependency
{
    public static Dependency<ICrmUserApi> UseCrmUserApi<TSqlApi>(this Dependency<TSqlApi> dependency)
        where TSqlApi : ISqlQueryEntitySetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<ICrmUserApi>(CreateApi);

        static CrmUserApi CreateApi(TSqlApi sqlApi)
        {
            ArgumentNullException.ThrowIfNull(sqlApi);
            return new(sqlApi); 
        }
    }
}