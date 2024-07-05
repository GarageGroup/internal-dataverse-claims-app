using System;

namespace GarageGroup.Internal.Dataverse.Claims;

public sealed record class DbUserGetIn
{
    public DbUserGetIn(Guid azureUserId)
        =>
        AzureUserId = azureUserId;

    public Guid AzureUserId { get; }
}