using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public readonly record struct CrmUserSetGetOut
{
    public required FlatArray<CrmUserSetGetItem> Users { get; init; }
}