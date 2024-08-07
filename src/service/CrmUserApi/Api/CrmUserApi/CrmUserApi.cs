﻿using GarageGroup.Infra;

namespace GarageGroup.Internal.Dataverse.Claims;

internal sealed partial class CrmUserApi(ISqlQueryEntitySetSupplier sqlApi) : ICrmUserApi
{
    private static readonly DbSelectQuery AllDbUserRequest
        =
        DbUser.QueryAll with
        {
            Filter = DbUser.DefaultFilter
        };
}