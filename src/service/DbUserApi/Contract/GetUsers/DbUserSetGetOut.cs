using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public readonly record struct DbUserSetGetOut
{
    public required FlatArray<DbUserSetGetItem> Users { get; init; }
}