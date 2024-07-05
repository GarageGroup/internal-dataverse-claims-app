﻿using System;
using PrimeFuncPack;

namespace GarageGroup.Internal.Dataverse.Claims;

public static class UsersSyncDependency
{
    public static Dependency<IUsersSyncHandler> UseUsersSyncHandler<TCrmUserApi>(
        this Dependency<TCrmUserApi, IDbUserApi> dependency)
        where TCrmUserApi : ICrmUserSetGetSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Fold<IUsersSyncHandler>(CreateHandler);

        static UsersSyncHandler CreateHandler(TCrmUserApi crmUserApi, IDbUserApi dbUserApi)
        {
            ArgumentNullException.ThrowIfNull(crmUserApi);
            ArgumentNullException.ThrowIfNull(dbUserApi);

            return new(crmUserApi, dbUserApi);
        }
    }
}