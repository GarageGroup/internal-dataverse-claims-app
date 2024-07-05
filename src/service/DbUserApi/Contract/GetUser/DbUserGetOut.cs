using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class DbUserGetOut
{
    public DbUserGetOut(Guid dataverseUserId)
        =>
        DataverseUserId = dataverseUserId;

    public Guid DataverseUserId { get; }
}